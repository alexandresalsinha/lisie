using ClassLibrary1;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.AspNet.SignalR.StockTicker
{
    public class StockTicker
    {
        // Singleton instance
        private readonly static Lazy<StockTicker> _instance = new Lazy<StockTicker>(
            () => new StockTicker(GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));

        private readonly object _marketStateLock = new object();
        private readonly object _updateStockPricesLock = new object();

        private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();

        // Stock can go up or down by a percentage of this factor on each change
        private readonly double _rangePercent = 0.002;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();

        private Timer _timer;
        private volatile bool _updatingStockPrices;
        private volatile MarketState _marketState;

        private StockTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
            LoadDefaultStocks();
        }

        public static StockTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public MarketState MarketState
        {
            get { return _marketState; }
            private set { _marketState = value; }
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }

        public void OpenMarket()
        {
            lock (_marketStateLock)
            {
                if (MarketState != MarketState.Open)
                {
                    _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);

                    MarketState = MarketState.Open;

                    BroadcastMarketStateChange(MarketState.Open);
                }
            }
        }

        public void CloseMarket()
        {
            lock (_marketStateLock)
            {
                if (MarketState == MarketState.Open)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                    }

                    MarketState = MarketState.Closed;

                    BroadcastMarketStateChange(MarketState.Closed);
                }
            }
        }

        public void Reset()
        {
            lock (_marketStateLock)
            {
                if (MarketState != MarketState.Closed)
                {
                    throw new InvalidOperationException("Market must be closed before it can be reset.");
                }

                LoadDefaultStocks();
                BroadcastMarketReset();
            }
        }

        private void LoadDefaultStocks()
        {
            _stocks.Clear();

            var stocks = new List<Stock>
            {
                new Stock { Symbol = "MSFT", Price = 41.68m },
                new Stock { Symbol = "AAPL", Price = 92.08m },
                new Stock { Symbol = "GOOG", Price = 543.01m }
            };

            stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));
        }

        private void UpdateStockPrices(object state)
        {
            // This function must be re-entrant as it's running as a timer interval handler
            lock (_updateStockPricesLock)
            {
                if (!_updatingStockPrices)
                {
                    _updatingStockPrices = true;

                    foreach (var stock in _stocks.Values)
                    {
                        if (TryUpdateStockPrice(stock))
                        {
                            BroadcastStockPrice(stock);
                        }
                    }

                    _updatingStockPrices = false;
                }
            }
        }

        private bool TryUpdateStockPrice(Stock stock)
        {
            // Randomly choose whether to udpate this stock or not
            var r = _updateOrNotRandom.NextDouble();
            if (r > 0.1)
            {
                return false;
            }

            // Update the stock price by a random factor of the range percent
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble() * _rangePercent;
            var pos = random.NextDouble() > 0.51;
            var change = Math.Round(stock.Price * (decimal)percentChange, 2);
            change = pos ? change : -change;

            stock.Price += change;
            return true;
        }

        private void BroadcastMarketStateChange(MarketState marketState)
        {
            switch (marketState)
            {
                case MarketState.Open:
                    Clients.All.marketOpened();
                    break;
                case MarketState.Closed:
                    Clients.All.marketClosed();
                    break;
                default:
                    break;
            }
        }

        public void BroadcastUpdateShoppingCartProductsInQueue(string valueString, string userId)
        {
            //HttpRequest request = HttpContext.Current.Request;

            //string userId = Microsoft.AspNet.Identity.User.Identity.GetUserId();

            //string _currentUserId = string.Empty;

            //try
            //{
            //    _currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //}
            //catch (Exception)
            //{
            //}

            //if (string.IsNullOrEmpty(_currentUserId))
            //{
            //Clients.All.updateShoppingCart(valueString);
            //}
            //else
            //{
            Clients.User(userId.ToString()).updateShoppingCartProductsInQueue(valueString);
            //}

            //Clients.All.updateShoppingCart(valueString);
        }

        public void BroadcastUpdateShoppingCart(int productListId, string userId, string deviceSource = "")
        {

            Clients.User(userId.ToString()).updateShoppingCart((productListId != -1) ? productListId.ToString() : "");
        }

        public void BroadcastNewPlayingSong(string userId)
        {
            Clients.User(userId.ToString()).newPlayingSong();
        }

        public void KeepConnectionAlive()
        {
            return;
        }

        private void BroadcastMarketReset()
        {
            Clients.All.marketReset();
        }


        private void BroadcastStockPrice(Stock stock)
        {
            Clients.All.updateStockPrice(stock);
        }

        public void BroadcastNewInteractions(string userId, int totalInteractions)
        {
            Clients.User(userId.ToString()).updateTotalInteractions(totalInteractions.ToString());
        }

        public void BroadcastNewProduct(string userId, int totalProducts)
        {
            Clients.User(userId.ToString()).updateTotalProducts(totalProducts);
        }

        public void BroadcastNewStoreProduct(string userId, int totalStoreProducts)
        {
            Clients.User(userId.ToString()).updateTotalStoreProducts(totalStoreProducts);
        }

        public void BroadcastStoreProductUpdate(string userId, StoreProducts storeProduct)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _query = from _prod in db.Products
                             join _store in db.Stores on storeProduct.StoreId equals _store.Id
                             where _prod.Id == storeProduct.ProductId.Value
                             select new SpiroWeb.Models.ProductsUpdatesModel
                             {
                                 ProductId = _prod.Id,
                                 Name = _prod.Name,
                                 Brand = _prod.Brand,
                                 Store = _store.Name,
                                 UpdateDate = storeProduct.UpdateDate.HasValue ? storeProduct.UpdateDate.Value : DateTime.MinValue,
                                 NeedsUpdate = storeProduct.NeedsUpdate.HasValue ? storeProduct.NeedsUpdate.Value : true,
                                 StoreUrl = _store.Url + storeProduct.Url
                             };
                var _list = _query.FirstOrDefault();
                if (_list != null)
                {
                    var _ProductPricesUpdate = db.ProductPricesUpdates.Where(c => c.ProductId == _list.ProductId).OrderByDescending(c => c.CreateDate).FirstOrDefault();
                    if (_ProductPricesUpdate != null)
                    {
                        _list.OldPrice = Math.Round(_ProductPricesUpdate.OldPrice, 2).ToString();
                        _list.NewPrice = Math.Round(_ProductPricesUpdate.NewPrice, 2).ToString();
                        _list.PriceUpdateDate = _ProductPricesUpdate.CreateDate;
                    }
                }

                Clients.User(userId.ToString()).updateStoreProductUpdate(_list);
            }
        }

        public void BroadcastStoreProductPriceUpdate(string userId, ProductPricesUpdates priceUpdate)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _query =
                    from _prod in db.Products
                    join _store in db.Stores on priceUpdate.StoreId equals _store.Id
                    where _prod.Id == priceUpdate.ProductId
                    select new SpiroWeb.Models.ProductsUpdatesModel
                    {
                        ProductId = _prod.Id,
                        Name = _prod.Name,
                        Brand = _prod.Brand,
                        Store = _store.Name,
                        StoreId = _store.Id,
                        PriceUpdateDate = priceUpdate.CreateDate,
                        //NeedsUpdate = _storeProduct.NeedsUpdate.HasValue ? _storeProduct.NeedsUpdate.Value : true,
                        StoreUrl = _store.Url,
                        OldPrice = Math.Round(priceUpdate.OldPrice, 2).ToString(),
                        NewPrice = Math.Round(priceUpdate.NewPrice, 2).ToString()
                    };
                var _item = _query.FirstOrDefault();
                if (_item != null)
                {
                    var _storeProduct = db.StoreProducts.Where(c => c.ProductId == _item.ProductId && c.StoreId == _item.StoreId).FirstOrDefault();
                    if (_storeProduct != null)
                    {
                        _item.NeedsUpdate = _storeProduct.NeedsUpdate.HasValue ? _storeProduct.NeedsUpdate.Value : true;
                        _item.StoreUrl += _storeProduct.Url;
                        _item.UpdateDate = _storeProduct.UpdateDate.HasValue ? _storeProduct.UpdateDate.Value : DateTime.MinValue;
                    }
                }

                Clients.User(userId.ToString()).updateStoreProductPricesUpdate(_item);
            }
        }

        public void BroadcastInteraction(string userId, Interactions interaction)
        {
            Clients.User(userId.ToString()).updateAllInteractions(interaction);
        }

        public void BroadcastNewUser(string userId, int totalUsers)
        {
            Clients.User(userId.ToString()).updateTotalUsers(totalUsers);
        }
    }

    public enum MarketState
    {
        Closed,
        Open
    }
}
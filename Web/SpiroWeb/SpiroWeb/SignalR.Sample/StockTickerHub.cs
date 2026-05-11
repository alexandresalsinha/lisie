using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.StockTicker
{
    [HubName("stockTicker")]
    public class StockTickerHub : Hub
    {
        private readonly StockTicker _stockTicker;

        public StockTickerHub() :
            this(StockTicker.Instance)
        {

        }

        public StockTickerHub(StockTicker stockTicker)
        {
            _stockTicker = stockTicker;
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stockTicker.GetAllStocks();
        }

        public string GetMarketState()
        {
            return _stockTicker.MarketState.ToString();
        }

        public void SendMeTheTime()
        {
            Clients.Caller.logServerTime(DateTime.Now.ToString());
        }

        public void OpenMarket()
        {
            _stockTicker.OpenMarket();
        }

        public void CloseMarket()
        {
            _stockTicker.CloseMarket();
        }

        public void KeepConnectionAlive()
        {
            _stockTicker.KeepConnectionAlive();
        }

        public void Reset()
        {
            _stockTicker.Reset();
        }
    }
}
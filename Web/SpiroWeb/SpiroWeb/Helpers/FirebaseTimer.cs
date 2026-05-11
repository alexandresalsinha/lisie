using ClassLibrary1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace SpiroWeb.Helpers
{
    public class FirebaseTimer
    {
        private BackgroundWorker worker;
        SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        public void Timer()
        {
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            Timer timer = new Timer(10000);// 1800000 30 minutos
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }
        int count = 0;
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Go Get to Firebase
            string json = FirebaseHelper.Helper.Get("SpiroStockManagement");

            //Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart("Product Received " + count, "d3d48305-4527-49ac-a930-49e4a511af14");
            //count++;
            //return;

            Dictionary<string, string> _tempJson = new Dictionary<string, string>();
            if (json == "null") return;

            JObject objsTemp = JObject.Parse(json);




            foreach (var item in objsTemp)
            {
                string key = item.Key.ToString();
                string value = item.Value.ToString();
                _tempJson.Add(key, value);
            }



            foreach (var item in _tempJson)
            {
                FirebaseItem _itemToInsert = JsonConvert.DeserializeObject<FirebaseItem>(item.Value);

                //Export to Sql Database (ProductQueue)
                //check if BarCode Exists in Products Database

                var query = (from c in db.Products
                             where c.Barcode.ToString() == _itemToInsert.barCode
                             select new
                             {
                                 c.Id
                             }).FirstOrDefault();
                if (query != null)
                {
                    //product exists add to UserProductsLists

                    //check if already exists in a user product list
                    UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                              where c.ProductId.Equals(query.Id) &&
                                                              c.UserId.Equals("d3d48305-4527-49ac-a930-49e4a511af14") &&
                                                              c.ListName.Equals("In")
                                                              select c).FirstOrDefault();

                    //Exist in User Lists , change quantity
                    if (queryExistsInUserList != null)
                    {
                        //int newQuantity = queryExistsInUserList.Quantity + 1;
                        queryExistsInUserList.Quantity = queryExistsInUserList.Quantity + 1;
                        db.UserProductsList.Attach(queryExistsInUserList);
                        var entry = db.Entry(queryExistsInUserList);
                        entry.Property(y => y.Quantity).IsModified = true;
                        // other changed properties
                        db.SaveChanges();

                        Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(queryExistsInUserList.Id, "d3d48305-4527-49ac-a930-49e4a511af14");
                    }
                    //add new product to user In List
                    else
                    {
                        UserProductsList _UserProductsList = new UserProductsList();
                        _UserProductsList.ProductId = query.Id;
                        _UserProductsList.UserId = "d3d48305-4527-49ac-a930-49e4a511af14";
                        _UserProductsList.Quantity = 1;
                        _UserProductsList.ListName = "In";

                        db.UserProductsList.Add(_UserProductsList);
                        db.SaveChanges();

                        Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(_UserProductsList.Id, "d3d48305-4527-49ac-a930-49e4a511af14");
                    }

                }
                //else add to ProductQueue
                else
                {
                    UserProductsQueue _UserProductsQueue = new UserProductsQueue();
                    _UserProductsQueue.BarCode = Convert.ToInt64(_itemToInsert.barCode);
                    _UserProductsQueue.ListName = "In";
                    _UserProductsQueue.UserId = "d3d48305-4527-49ac-a930-49e4a511af14";
                    _UserProductsQueue.IsRegistered = false;

                    db.UserProductsQueue.Add(_UserProductsQueue);
                    db.SaveChanges();

                    //Brodcast new product in queue
                    Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCartProductsInQueue("Product Received " + count, "d3d48305-4527-49ac-a930-49e4a511af14");
                }

                FirebaseHelper.Helper.Delete(item.Key);
                return;

                //Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastProductsDatabaseChange();
            }
        }
    }

}
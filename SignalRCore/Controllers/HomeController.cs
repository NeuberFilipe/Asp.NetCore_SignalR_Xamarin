using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microcharts;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Mvc;
using SignalRCore.Models;
using SkiaSharp;

namespace SignalRCore.Controllers
{
    public class HomeController : Controller
    {

        #region Properties
        protected IHubProxy _hub;
        private string url = "http://localhost:53681";
        protected HubConnection connection;
        private bool _reconnect = true;
        private readonly int _interval = 5000;
        private const string _nameHub = "MicrochartHub"; 
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult Microchart()
        {
            return View();
        }


        public ActionResult SaveMicrochart(EntryModel entryModel)
        {
            while (_reconnect)
            {
                ConnectAsync();

                if (!_reconnect)
                {
                    Thread.Sleep(_interval);
                }
                List<Entry> list = new List<Entry>();
                if (ModelState.IsValid)
                {

                    try
                    {
                        list.Add(new Entry(Convert.ToInt32(entryModel.ValueLabel))
                        {
                            Color = SKColor.Parse(entryModel.Color),
                            Label = entryModel.Label,
                            ValueLabel = entryModel.ValueLabel
                        });

                        if (connection.State == ConnectionState.Connected || connection !=null)
                        {
                            _hub.Invoke("SendMicrochartSignal", list);
                        }
                        else
                        {
                            _reconnect = true;
                        }

                          
                    }
                    catch (Exception ex)
                    {
                        _reconnect = true;
                        Thread.Sleep(_interval);
                    } 
                }
            }

            return View("Microchart", entryModel);
        }

        private async void ConnectAsync()
        {
            Task.Run(() => { StartConnection(); });
        }

        private void StartConnection()
        {
            while (true)
            {
                if (!_reconnect)
                {
                    Thread.Sleep(_interval);
                    continue;
                }

                connection = new HubConnection(url);
                _hub = connection.CreateHubProxy(_nameHub);

                try
                {
                    connection.Start().ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                        }
                        else
                        {
                        }
                    });
                    _reconnect = false;
                }
                catch (Exception ex)
                {
                    _reconnect = true;
                    Thread.Sleep(_interval);
                }
            }
        }
    }
}

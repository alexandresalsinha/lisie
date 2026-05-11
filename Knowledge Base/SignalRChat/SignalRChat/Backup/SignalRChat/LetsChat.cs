using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRChat
{
    [HubName("myChatHub")]
    public class LetsChat : Hub
    {
        public void send(string message)
        {
            Clients.All.addMessage(message);
        }
    }
}
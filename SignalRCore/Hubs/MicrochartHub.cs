using System.Collections.Generic;
using Microcharts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;

namespace SignalRCore.Hubs
{
    [HubName("MicrochartHub")]
    public class MicrochartHub : Hub
    {
        public void Join()
        {
            Clients.All.Join($"{Context.ConnectionId} has joined to room");
        }

        public void SendMicrochartSignal(List<Entry> signalValue)
        {
            Clients.All.InvokeAsync("UpdateMicrochart", signalValue);
        }
    }
}

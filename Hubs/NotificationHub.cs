using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebDekAPI.Hubs
{
    public class NotificationHub : Hub
    {
		public ConnectionMapping<string> _connections =
			new ConnectionMapping<string>();
		public NotificationHub(ConnectionMapping<string> connectionMapping)
		{
			_connections = connectionMapping;
		}
		
		public async Task Send(string message)
        {
			int userID = authService.GetUserID(Context.User);
			await this.Clients.Caller.SendAsync("Send", "Добро пожаловать! Пользователей онлайн: " + _connections.Count);
			

        }
		public override Task OnConnectedAsync()
		{
			if (Context.User.Identity.Name != null)
			{
				string userID = authService.GetUserID(Context.User).ToString();

				_connections.Add(userID, Context.ConnectionId);
			}

			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			string userID = authService.GetUserID(Context.User).ToString();

			_connections.Remove(userID, Context.ConnectionId);
			return base.OnDisconnectedAsync(exception);
		}
	}
}

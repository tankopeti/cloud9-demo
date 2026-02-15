using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Cloud9_2.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendPrivateMessage(string toUser, string message)
        {
            var sender = Context.User?.Identity?.Name ?? "Unknown";
            var receiverConnectionId = UserHandler.ConnectedUsers
                .FirstOrDefault(u => u.Key == toUser).Value;

            if (receiverConnectionId != null)
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", sender, message, false);
                await Clients.Caller.SendAsync("ReceivePrivateMessage", sender, message, false);
            }
        }

        public async Task SendGroupMessage(string message)
        {
            var sender = Context.User?.Identity?.Name ?? "Unknown";
            await Clients.All.SendAsync("ReceivePrivateMessage", sender, message, true);
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                UserHandler.ConnectedUsers[username] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                UserHandler.ConnectedUsers.Remove(username);
            }
            return base.OnDisconnectedAsync(exception);
        }

            // New video call methods
        public async Task InitiateVideoCall(string recipient)
        {
            await Clients.User(recipient).SendAsync("ReceiveVideoCall", Context.User.Identity.Name);
        }

        public async Task AcceptVideoCall(string caller)
        {
            // Create and send an answer
            await Clients.User(caller).SendAsync("ReceiveAnswer", "accepted");
        }

        public async Task RejectVideoCall(string caller)
        {
            await Clients.User(caller).SendAsync("ReceiveAnswer", "rejected");
        }

        public async Task SendIceCandidate(string recipient, string candidate)
        {
            await Clients.User(recipient).SendAsync("ReceiveIceCandidate", candidate);
        }

        public async Task SendAnswer(string recipient, string answer)
        {
            await Clients.User(recipient).SendAsync("ReceiveAnswer", answer);
        }

        public async Task EndVideoCall(string recipient)
        {
            await Clients.User(recipient).SendAsync("CallEnded");
        }

        // Helper method to get connection ID for a user
        private async Task<string> GetConnectionIdForUser(string username)
        {
            // In a real application, you'd want to track user connections properly
            // This is a simplified version
            return Context.ConnectionId;
        }

    }

    public static class UserHandler
    {
        public static Dictionary<string, string> ConnectedUsers { get; set; } = new Dictionary<string, string>();
    }
}

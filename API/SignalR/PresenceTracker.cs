using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        /*
            for every device that a user connects to the server with, the user will be given
            a different connection id.

            here, we store them as an array under a username key with a dictionary.

            dictionary, however, is not a thread-safe resource. if we have concurrent users
            trying to connect and update it at the same time, we will run into problems.

            thus, we need to lock it whenever the connection method is called.
        */
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    var userConnections = OnlineUsers[username];

                    if (!userConnections.Contains(connectionId))
                    {
                        userConnections.Add(connectionId);
                    }
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> { connectionId });
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    var userConnections = OnlineUsers[username];

                    if (userConnections.Contains(connectionId))
                    {
                        userConnections.Remove(connectionId);
                    }

                    if (userConnections.Count == 0)
                    {
                        OnlineUsers.Remove(username);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;

            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers
                    .OrderBy(k => k.Key)
                    .Select(k => k.Key)
                    .ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
    }
}
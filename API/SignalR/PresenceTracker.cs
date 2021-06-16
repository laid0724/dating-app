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

        public Task<bool> UserConnected(string username, string connectionId)
        {
            bool isOnline = false;
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
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;
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
                        isOffline = true;
                    }
                }
            }

            return Task.FromResult(isOffline);
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

        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            
            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }
}
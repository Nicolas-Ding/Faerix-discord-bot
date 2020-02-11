using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace BJR_bot.Services
{
    public class LolService
    {
        private Dictionary<string, Dictionary<IUser, DispoData>> usersLookingForGame; 
 
        public LolService()
        {
            usersLookingForGame = new Dictionary<string, Dictionary<IUser, DispoData>>();
        }

        public Dictionary<IUser, DispoData> GetUsers(string game)
        {
            game = game.ToLower();
            CleanList();
            return usersLookingForGame.ContainsKey(game) ? usersLookingForGame[game] : new Dictionary<IUser, DispoData>();
        }

        public void AddUser(string game, IUser user, TimeSpan availableTime)
        {
            AddUser(game, user, availableTime, DateTimeOffset.UtcNow);
        }

        public void AddUser(string game, IUser user, TimeSpan availableTime, DateTimeOffset startTime)
        {
            game = game.ToLower();
            if (!usersLookingForGame.ContainsKey(game))
            {
                usersLookingForGame[game] = new Dictionary<IUser, DispoData>();
            }

            usersLookingForGame[game][user] = new DispoData
            {
                StartTime = startTime, 
                EndTime = startTime.Add(availableTime)
            };
        }

        public void RemoveUser(string game, IUser user)
        {
            game = game.ToLower();
            usersLookingForGame[game].Remove(user);
        }


        public void RemoveUser(IUser user)
        {
            var keys = new List<string>(usersLookingForGame.Keys);

            foreach (string game in keys)
            {
                RemoveUser(game, user);
            }
        }

        public void CleanList()
        {
            var keys = new List<string>(usersLookingForGame.Keys);

            foreach (string game in keys)
            {
                usersLookingForGame[game] = usersLookingForGame[game]
                    .Where(_ => _.Value.EndTime > DateTimeOffset.UtcNow)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }
    }
}

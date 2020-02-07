using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace BJR_bot.Services
{
    public class LolService
    {
        private Dictionary<string, Dictionary<IUser, DateTimeOffset>> usersLookingForGame; 
 
        public LolService()
        {
            usersLookingForGame = new Dictionary<string, Dictionary<IUser, DateTimeOffset>>();
        }

        public Dictionary<IUser, DateTimeOffset> GetUsers(string game)
        {
            CleanList();
            return usersLookingForGame.ContainsKey(game) ? usersLookingForGame[game] : new Dictionary<IUser, DateTimeOffset>();
        }

        public void AddUser(string game, IUser user)
        {
            if (!usersLookingForGame.ContainsKey(game))
            {
                usersLookingForGame[game] = new Dictionary<IUser, DateTimeOffset>();
            }

            usersLookingForGame[game][user] = DateTimeOffset.UtcNow.Add(TimeSpan.FromHours(1));
        }

        public void RemoveUser(string game, IUser user)
        {
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
                    .Where(_ => _.Value > DateTimeOffset.UtcNow)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }
    }
}

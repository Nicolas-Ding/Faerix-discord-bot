using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace BJR_bot.Services
{
    public class LolService
    {
        private Dictionary<string, IList<(DateTimeOffset, IUser)>> usersLookingForGame; 
 
        public LolService()
        {
            usersLookingForGame = new Dictionary<string, IList<(DateTimeOffset, IUser)>>();
        }

        public IList<(DateTimeOffset, IUser)> GetUsers(string game)
        {
            CleanList();
            return usersLookingForGame.ContainsKey(game) ? usersLookingForGame[game] : new List<(DateTimeOffset, IUser)>();
        }

        public void AddUser(string game, IUser user)
        {
            if (!usersLookingForGame.ContainsKey(game))
            {
                usersLookingForGame[game] = new List<(DateTimeOffset, IUser)>();
            }

            if (usersLookingForGame[game].All(_ => _.Item2.Id != user.Id))
            {
                usersLookingForGame[game].Add((DateTimeOffset.UtcNow.Add(TimeSpan.FromHours(1)), user));
            }
        }

        public void RemoveUser(string game, IUser user)
        {
            usersLookingForGame[game] = usersLookingForGame[game]
                .Where(_ => _.Item2.Id != user.Id)
                .ToList();
        }


        public void RemoveUser(IUser user)
        {
            List<string> keys = new List<string>(usersLookingForGame.Keys);

            foreach (string game in keys)
            {
                RemoveUser(game, user);
            }
        }

        public void CleanList()
        {
            List<string> keys = new List<string>(usersLookingForGame.Keys);

            foreach (string game in keys)
            {
                usersLookingForGame[game] = usersLookingForGame[game]
                    .Where(_ => _.Item1 > DateTimeOffset.UtcNow)
                    .ToList();
            }
        }
    }
}

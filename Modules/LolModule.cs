using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BJR_bot.Services;
using BJR_bot.TypeReaders;
using Discord;
using Discord.Commands;

namespace BJR_bot.Modules
{
    public class LolModule : ModuleBase<SocketCommandContext>
    {
        private readonly LolService _lolService;
        

        public LolModule(LolService lolService)
        {
            _lolService = lolService;
        }

        [Command("dispo")]
        public async Task DispoAsync(string game)
        {
            await DispoAsync(game, TimeSpan.FromHours(1));
        }

        [Command("dispo")]
        public async Task DispoAsync(string game, TimeSpan timespan)
        {
            if (timespan > TimeSpan.FromHours(12))
            {
                await ReplyAsync(
                    $"Désolé {Context.User.Mention}, tu ne peux pas être dispo pendant plus de 12h de suite :)");
                return;
            }
            _lolService.AddUser(game, Context.User, timespan);
            await ReplyAsync($"Merci {Context.User.Mention}, j'ai noté que tu es dispo pour jouer à **{game}** pour {timespan:h'h 'm'm'}");
        }

        [Command("dispo")]
        public async Task DispoAsync(string game, InKeywordType _, TimeSpan delay)
        {
            await DispoAsync(game, TimeSpan.FromHours(1), _, delay);
        }

        [Command("dispo")]
        public async Task DispoAsync(string game, TimeSpan timespan, InKeywordType _, TimeSpan delay)
        {
            if (timespan > TimeSpan.FromHours(12))
            {
                await ReplyAsync(
                    $"Désolé {Context.User.Mention}, tu ne peux pas être dispo pendant plus de 12h de suite :)");
                return;
            }
            _lolService.AddUser(game, Context.User, timespan, DateTimeOffset.UtcNow.Add(delay));
            await ReplyAsync($"Merci {Context.User.Mention}, j'ai noté que tu es dispo pour jouer à **{game}** pour {timespan:h'h 'm'm'} dans {delay:h'h 'm'm'}");
        }

        [Command("plusdispo")]
        [Alias("pasdispo")]
        public async Task PlusDispoAsync(string game)
        {
            _lolService.RemoveUser(game, Context.User);
            await ReplyAsync($"Merci {Context.User.Mention}, j'ai noté que tu n'es plus dispo pour jouer à **{game}**");
        }

        [Command("plusdispo")]
        [Alias("pasdispo")]
        public async Task PlusDispoAsync()
        {
            _lolService.RemoveUser(Context.User);
            await ReplyAsync($"Merci {Context.User.Mention}, j'ai noté que tu n'es plus dispo pour jouer");
        }

        [Command("list")]
        public async Task ListAsync(string game)
        {
            Dictionary<IUser, DispoData> dispoUsers = _lolService.GetUsers(game);
            EmbedBuilder embed;
            if (dispoUsers.Count == 0)
            {
                embed = new EmbedBuilder
                {
                    Color = Color.Red
                };
                embed.AddField(game, $"Désolé, personne ne veut jouer à **{game}** pour l'instant");
            }
            else
            {
                embed = new EmbedBuilder
                {
                    Description = $"Voici la liste des personnes disponibles pour jouer à **{game}** : \n", 
                    Color = Color.Blue
                };
                var dispoMessage = new StringBuilder();
                var bientotDispoMessage = new StringBuilder();

                foreach (var dispoUser in dispoUsers)
                {
                    if (dispoUser.Value.StartTime < DateTimeOffset.UtcNow)
                    {
                        dispoMessage.Append(
                            $"**{dispoUser.Key.Username}** est encore dispo {dispoUser.Value.EndTime - DateTimeOffset.UtcNow:h'h 'm'm'}\n");
                    }
                    else
                    {
                        bientotDispoMessage.Append(
                            $"**{dispoUser.Key.Username}** sera dispo dans {DateTime.UtcNow - dispoUser.Value.StartTime:h'h 'm'm'}\n");
                    }
                }

                if (dispoMessage.Length > 0)
                {
                    embed.AddField($"Disponible maintenant :", dispoMessage.ToString());
                }
                if (bientotDispoMessage.Length > 0)
                {
                    embed.AddField($"Disponible bientôt :", bientotDispoMessage.ToString());
                }
            }

            await ReplyAsync(embed: embed.Build());
        }

        [Command("ping")]
        public async Task PingAsync(string game)
        {
            Dictionary<IUser, DispoData> dispoUsers = _lolService.GetUsers(game);
            StringBuilder message = new StringBuilder($"Ping pour jouer à **{game}**\n");
            foreach (var dispoUser in dispoUsers)
            {
                message.Append($"{dispoUser.Key.Mention}\tEncore {dispoUser.Value.EndTime - DateTimeOffset.UtcNow:h'h 'm'm 's's'}\n");
            }
            await ReplyAsync(message.ToString());
        }
    }
}

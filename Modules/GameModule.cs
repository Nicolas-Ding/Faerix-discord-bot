using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BJR_bot.Services;
using Discord;
using Discord.Commands;

namespace BJR_bot.Modules
{
    public class GameModule : ModuleBase<SocketCommandContext>
    {
        private readonly GameService _gameService;


        public GameModule(GameService gameService)
        {
            _gameService = gameService;
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
            _gameService.AddUser(game, Context.User, timespan);
            await ReplyAsync($"Merci {Context.User.Mention}, j'ai noté que tu es dispo pour jouer à **{game}** pour {timespan:h'h 'm'm 's's'}");
        }

        [Command("plusdispo")]
        [Alias("pasdispo")]
        public async Task PlusDispoAsync(string game)
        {
            _gameService.RemoveUser(game, Context.User);
            await ReplyAsync($"Merci {Context.User.Mention}, j'ai noté que tu n'es plus dispo pour jouer à **{game}**");
        }

        [Command("plusdispo")]
        [Alias("pasdispo")]
        public async Task PlusDispoAsync()
        {
            _gameService.RemoveUser(Context.User);
            await ReplyAsync($"Merci {Context.User.Mention}, j'ai noté que tu n'es plus dispo pour jouer");
        }

        [Command("list")]
        public async Task ListAsync(string game)
        {
            Dictionary<IUser, DateTimeOffset> dispoUsers = _gameService.GetUsers(game);
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
                    Description = $"Voici la liste des personnes disponibles pour jouer à **{game}**\n",
                    Color = Color.Blue
                };
                var message = new StringBuilder();
                foreach (var dispoUser in dispoUsers)
                {
                    embed.AddField(dispoUser.Key.Username,
                        $"Encore {dispoUser.Value - DateTimeOffset.UtcNow:h'h 'm'm 's's'}");
                }
            }

            await ReplyAsync(embed: embed.Build());
        }

        [Command("ping")]
        public async Task PingAsync(string game)
        {
            Dictionary<IUser, DateTimeOffset> dispoUsers = _gameService.GetUsers(game);
            StringBuilder message = new StringBuilder($"Ping pour jouer à **{game}**\n");
            foreach (var dispoUser in dispoUsers)
            {
                message.Append($"{dispoUser.Key.Mention}\tEncore {dispoUser.Value - DateTimeOffset.UtcNow:h'h 'm'm 's's'}\n");
            }
            await ReplyAsync(message.ToString());
        }
    }
}

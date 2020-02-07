using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BJR_bot.Services;
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

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("dispo")]
        public async Task DispoAsync(string game)
        {
            _lolService.AddUser(game, Context.User);
            await ReplyAsync($"Merci {Context.User.Mention}, j'ai noté que tu es dispo pour jouer à **{game}** pour 1h");
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
           Dictionary<IUser, DateTimeOffset> dispoUsers = _lolService.GetUsers(game);
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
                        $"Encore {(int) (dispoUser.Value - DateTimeOffset.UtcNow).TotalMinutes} min");
                }
            }

            await ReplyAsync(embed: embed.Build());
        }

        [Command("ping")]
        public async Task PingAsync(string game)
        {
            _lolService.AddUser(game, Context.User);
            Dictionary<IUser, DateTimeOffset> dispoUsers = _lolService.GetUsers(game);
            StringBuilder message = new StringBuilder($"Ping pour jouer à **{game}**\n");
            foreach (var dispoUser in dispoUsers)
            {
                message.Append($"{dispoUser.Key.Mention}\tEncore {(int)(dispoUser.Value - DateTimeOffset.UtcNow).TotalMinutes}min\n");
            }
            await ReplyAsync(message.ToString());
        }
    }
}

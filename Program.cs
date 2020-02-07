using System;
using System.Threading.Tasks;
using BJR_bot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;

namespace BJR_bot
{
    class Program
    {
        private DiscordSocketClient _client;

        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            var token = await GetTokenAsync();

            var services = ConfigureServices();
            _client = services.GetRequiredService<DiscordSocketClient>();

            _client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            _client.Ready += ReadyAsync;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
            services.Dispose();
        }

        private async Task<string> GetTokenAsync()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient =
                new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient
                .GetSecretAsync(
                    "https://discord-bot-keyvault.vault.azure.net/secrets/bot-token/f1b36f4771cf4671b9c0e43d963c625c")
                .ConfigureAwait(false);
            string token = secret.Value;
            return token;
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<LolService>()
                .BuildServiceProvider();
        }
    }
}
using System;
using System.Threading.Tasks;
using DiscordJuuBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;

namespace DiscordJuuBot
{
    class Program
    {
        static DiscordClient Discord;
        static CommandsNextModule Commands;
        static VoiceNextClient Voice;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Config.TOKEN,
                TokenType = TokenType.Bot
            });

            // Message Received
            Discord.MessageCreated += MessageCreated;

            // Register Commands
            Commands = Discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });
            Commands.RegisterCommands<BasicCommands>();

            // Register Voice
            Voice = Discord.UseVoiceNext();

            Console.WriteLine("Bot Giriş Yapıyor.");
            await Discord.ConnectAsync();
            Console.WriteLine("Bot Giriş Başarılı.");

            await Task.Delay(-1);
        }

        static async Task MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs messageEvent)
        {
            // You can check every message received here
        }
    }
}

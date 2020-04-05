using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;

namespace DiscordJuuBot.Commands
{
    public class BasicCommands
    {
        public const string BOT_URL = "https://discordapp.com/api/oauth2/authorize?client_id=696471288935350342&permissions=8&scope=bot";
        public VoiceNextClient voiceClient;

        [Command("selam")]
        public async Task Hi(CommandContext ctx)
        {
            await ctx.RespondAsync($"👋 Selam sana, {ctx.User.Mention}!");
        }

        [Command("corona")]
        public async Task Corona(CommandContext ctx)
        {
            Random random = new Random();

            string[] messagelist = new string[] {
                ":mask: Maskeni taktın mı yakışıklı.",
                ":sneezing_face: Hapşuuuuuuuu!",
                "Corona.exe başlatılıyor...",
            };
            await ctx.RespondAsync(messagelist[random.Next(messagelist.Length)]);
        }

        [Command("join")]
        public async Task Join(CommandContext ctx)
        {
            if (voiceClient == null) voiceClient = ctx.Client.GetVoiceNextClient();


            var vnc = voiceClient.GetConnection(ctx.Guild);
            if (vnc != null)
                throw new InvalidOperationException("Already connected in this guild.");

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                throw new InvalidOperationException("You need to be in a voice channel.");

            vnc = await voiceClient.ConnectAsync(chn);
            await ctx.RespondAsync("👌");
        }

        [Command("leave")]
        public async Task Leave(CommandContext ctx)
        {
            if (voiceClient == null) voiceClient = ctx.Client.GetVoiceNextClient();

            var vnc = voiceClient.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");

            vnc.Disconnect();
            await ctx.RespondAsync("👌");
        }

        [Command("play")]
        public async Task Play(CommandContext ctx, [RemainingText] string file)
        {
            if (voiceClient == null) voiceClient = ctx.Client.GetVoiceNextClient();

            var vnc = voiceClient.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");

            //if (!File.Exists(file))
            //    throw new FileNotFoundException("File was not found.");

            await ctx.RespondAsync("👌");
            await vnc.SendSpeakingAsync(true); // send a speaking indicator

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{file}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var ffmpeg = Process.Start(psi);
            var ffout = ffmpeg.StandardOutput.BaseStream;

            var buff = new byte[3840];
            var br = 0;
            while ((br = ffout.Read(buff, 0, buff.Length)) > 0)
            {
                if (br < buff.Length) // not a full sample, mute the rest
                    for (var i = br; i < buff.Length; i++)
                        buff[i] = 0;

                await vnc.SendAsync(buff, 20);
            }

            await vnc.SendSpeakingAsync(false); // we're not speaking anymore
        }
    }
}

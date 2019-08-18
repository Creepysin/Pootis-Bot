﻿using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Pootis_Bot.Core;
using Pootis_Bot.Services.Google;

namespace Pootis_Bot.Modules.Fun
{
    public class GoogleSearch : ModuleBase<SocketCommandContext>
    {
        // Module Information
        // Original Author   - Creepysin
        // Description      - Searches Google
        // Contributors     - Creepysin, 

        [Command("google")]
        [Summary("Searches Google")]
        [Alias("g")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task CmdGoogleSearch([Remainder]string search = "")
        {
            if (string.IsNullOrWhiteSpace(Config.bot.Apis.apiGoogleSearchKey) || 
                string.IsNullOrWhiteSpace(Config.bot.Apis.googleSearchEngineID))
            {
                await Context.Channel.SendMessageAsync("Google search is disabled by the bot owner.");
                return;
            }

            var searchListResponse = GoogleService.Search(search, this.GetType().ToString());

            StringBuilder results = new StringBuilder();

            int currentResult = 0;
            foreach(var result in searchListResponse.Items)
            {
                if(currentResult!= 5)
                {
                    results.Append($"[{result.Title}]({result.Link})\n{result.Snippet}\n");
                    currentResult += 1;
                }
            }

            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = $"Google Search For '{search}'";
            embed.WithDescription(results.ToString());
            embed.WithFooter($"Search by {Context.User} @ ", Context.User.GetAvatarUrl());
            embed.WithColor(FunCmdsConfig.googleColor);
            embed.WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}

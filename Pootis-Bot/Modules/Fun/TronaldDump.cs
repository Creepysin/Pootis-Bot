﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Pootis_Bot.Services.Fun;

namespace Pootis_Bot.Modules.Fun
{
    public class TronaldDump : ModuleBase<SocketCommandContext>
    {
        // Module Information
        // Original Author   - Creepysin
        // Description      - Uses the tronaldump api to get Tronal Dump quotes
        // Contributors     - Creepysin, 

        readonly string trumpImageUrl = "https://assets.tronalddump.io/img/tronalddump_850x850.png";

        [Command("tronald")]
        [Summary("Search Donald Trump quotes")]
        [Alias("tronalddump", "dump", "donald", "donaldtrump", "trump")]
        public async Task Tronald([Remainder] string subCmd = "random")
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithThumbnailUrl(trumpImageUrl);

            if(subCmd == "random")
            {
                embed.WithTitle("Random Trump Quote");
                embed.WithDescription(TronaldDumpService.GetRandomQuote());
            }
            else
            {
                embed.WithTitle("Donald Trump Quote Search");
                embed.WithDescription(TronaldDumpService.GetQuote(subCmd));
            }

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}

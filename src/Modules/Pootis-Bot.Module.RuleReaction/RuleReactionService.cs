﻿using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Pootis_Bot.Config;
using Pootis_Bot.Logging;
using Pootis_Bot.Module.RuleReaction.Entities;

namespace Pootis_Bot.Module.RuleReaction
{
    internal static class RuleReactionService
    {
        private static readonly RuleReactionConfig Config;
        
        static RuleReactionService()
        {
            Config ??= Config<RuleReactionConfig>.Instance;
        }

        public static async Task CheckAllServer(DiscordSocketClient client)
        {
            Logger.Debug("Checking rule reaction servers...");
            
            foreach (RuleReactionServer server in Config.RuleReactionServers)
            {
                await CheckServer(server, client, true);
            }
        }
        
        public static async Task<bool> CheckServer(RuleReactionServer server, DiscordSocketClient client, bool careAboutEnabled = false)
        {
            //Get guild
            SocketGuild guild = client.GetGuild(server.GuildId);
            if (guild == null)
            {
                Config.RuleReactionServers.Remove(server);
                Config.Save();
                
                Logger.Debug("Removed server {ServerId} from rule reaction as it no longer exists.", server.GuildId);
                return false;
            }
            
            if(careAboutEnabled)
                if (!server.Enabled)
                    return true;
            
            //Check the role
            SocketRole role = guild.GetRole(server.RoleId);
            if (role == null)
            {
                server.RoleId = 0;
                server.Enabled = false;
                Config.Save();
                
                Logger.Debug("Disabled server {ServerId} rule reaction as the role no longer exists.", server.GuildId);
                return false;
            }

            //Check that the channel still exists
            SocketTextChannel channel = guild.GetTextChannel(server.ChannelId);
            if (channel == null)
            {
                server.Enabled = false;
                server.ChannelId = 0;
                server.MessageId = 0;
                Config.Save();
                
                Logger.Debug("Disabled server {ServerId} rule reaction as the text channel no longer exists.", server.GuildId);
                return false;
            }
            
            //Check the message still exists
            IMessage message = await channel.GetMessageAsync(server.MessageId);
            if (message != null) return true;
            
            server.Enabled = false;
            server.MessageId = 0;
            Config.Save();
                
            Logger.Debug("Disabled server {ServerId} rule reaction as the message no longer exists.", server.GuildId);
            return false;
        }

        public static async Task ReactionAdded(ISocketMessageChannel channel, SocketReaction reaction, DiscordSocketClient client)
        {
            RuleReactionServer server = Config.RuleReactionServers.FirstOrDefault(x => x.MessageId == reaction.MessageId);
            if(server is not {Enabled: true})
                return;

            //If the emote is right, add the role
            if (reaction.Emote.Name == server.Emoji)
            {
                await client.GetGuild(server.GuildId).GetUser(reaction.UserId).AddRoleAsync(server.RoleId);
            }
        }
    }
}
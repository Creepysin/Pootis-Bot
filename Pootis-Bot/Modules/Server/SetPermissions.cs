﻿using System.Threading.Tasks;
using Discord.Commands;
using Pootis_Bot.Core.ServerList;

namespace Pootis_Bot.Modules.Server
{
    public class SetPermissions : ModuleBase<SocketCommandContext>
    {
        [Command("permmakenotwarnable")]
        public async Task PermNotWarnable([Remainder]string role = "")
        {
            if(role.Trim() == "")
            {
                await Context.Channel.SendMessageAsync("You need to set a role\nE.G: `permakenotwarnable Admin`");
                return;
            }

            var list = ServerLists.GetServer(Context.Guild);
            list.permNotWarnableRole = role;
            ServerLists.SaveServerList();

            await Context.Channel.SendMessageAsync($"Command `makenotwarnable` permission was set to '{role}'");
        }

        [Command("permakewarnable")]
        public async Task PermWarnable([Remainder]string role = "")
        {
            if (role.Trim() == "")
            {
                await Context.Channel.SendMessageAsync("You need to set a role\nE.G: `permakewarnable Admin`");
                return;
            }

            var list = ServerLists.GetServer(Context.Guild);
            list.permMakeWarnableRole = role;
            ServerLists.SaveServerList();

            await Context.Channel.SendMessageAsync($"Command `makewarnable` permission was set to '{role}'");
        }

        [Command("permwarn")]
        public async Task PermWarn([Remainder]string role = "")
        {
            if (role.Trim() == "")
            {
                await Context.Channel.SendMessageAsync("You need to set a role\nE.G: `permwarn Admin`");
                return;
            }

            var list = ServerLists.GetServer(Context.Guild);
            list.permWarn = role;
            ServerLists.SaveServerList();

            await Context.Channel.SendMessageAsync($"Command `warn` permission was set to '{role}'");
        }

        [Command("permgoogle")]
        public async Task PermGoogle([Remainder]string role = "")
        {
            if (role.Trim() == "")
            {
                await Context.Channel.SendMessageAsync("You need to set a role\nE.G: `permgoogle Admin`");
                return;
            }

            var list = ServerLists.GetServer(Context.Guild);
            list.permGoogle = role;
            ServerLists.SaveServerList();

            await Context.Channel.SendMessageAsync($"Command `google` permission was set to '{role}'");
        }

        [Command("permyoutube")]
        public async Task PermYoutube([Remainder]string role = "")
        {
            if (role.Trim() == "")
            {
                await Context.Channel.SendMessageAsync("You need to set a role\nE.G: `permyoutube Admin`");
                return;
            }

            var list = ServerLists.GetServer(Context.Guild);
            list.permYT = role;
            ServerLists.SaveServerList();

            await Context.Channel.SendMessageAsync($"Command `youtube` permission was set to '{role}'");
        }

        [Command("permgiphy")]
        public async Task PermGiphy([Remainder]string role = "")
        {
            if (role.Trim() == "")
            {
                await Context.Channel.SendMessageAsync("You need to set a role\nE.G: `permgiphy Admin`");
                return;
            }

            var list = ServerLists.GetServer(Context.Guild);
            list.permGiphy = role;
            ServerLists.SaveServerList();

            await Context.Channel.SendMessageAsync($"Command `giphy` permission was set to '{role}'");
        }
    }
}
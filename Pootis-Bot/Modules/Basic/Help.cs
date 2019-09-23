﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Pootis_Bot.Core;
using Pootis_Bot.Entities;

namespace Pootis_Bot.Modules.Basic
{
	public class Help : ModuleBase<SocketCommandContext>
	{
		// Module Information
		// Original Author   - Creepysin
		// Description      - The two help commands
		// Contributors     - Creepysin, 

		private readonly CommandService _cmdService;

		public Help(CommandService commandService)
		{
			_cmdService = commandService;
		}

		[Command("help")]
		[Alias("h")]
		[Summary("Gets help")]
		public async Task HelpCmd()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(
				$"```# Pootis-Bot Normal Commands```\nFor more help on a specific command do `{Global.BotPrefix}help [command]`.\n");

			//Basic Commands
			foreach (GlobalConfigFile.HelpModule helpModule in Config.bot.HelpModules)
			{
				builder.Append($"\n**{helpModule.Group}** - ");
				foreach (string module in helpModule.Modules)
				{
					foreach (CommandInfo cmd in GetModule(module).Commands) builder.Append($"`{cmd.Name}` ");
				}
			}

			await Context.Channel.SendMessageAsync(builder.ToString());
		}

		[Command("help")]
		[Alias("h", "command", "chelp", "ch")]
		[Summary("Gets help on a specific command")]
		public async Task HelpSpecific([Remainder] string query)
		{
			EmbedBuilder embed = new EmbedBuilder();
			embed.WithTitle($"Help for {query}");
			embed.WithColor(new Color(241, 196, 15));

			SearchResult result = _cmdService.Search(Context, query);
			if (result.IsSuccess)
				foreach (CommandMatch command in result.Commands)
					embed.AddField(command.Command.Name,
						$"Summary: {command.Command.Summary}\nAlias: {FormatAliases(command.Command)}\nUsage: `{command.Command.Name} {FormatParms(command.Command)}`");

			if (embed.Fields.Count == 0)
				embed.WithDescription("Nothing was found for " + query);

			await Context.Channel.SendMessageAsync("", false, embed.Build());
		}

		private string FormatAliases(CommandInfo commandInfo)
		{
			IReadOnlyList<string> aliases = commandInfo.Aliases;

			StringBuilder format = new StringBuilder();

			int count = aliases.Count;
			int currentCount = 1;
			foreach (string alias in aliases)
			{
				format.Append(alias);

				if (currentCount != count) format.Append(", ");
				currentCount += 1;
			}

			return format.ToString();
		}

		private string FormatParms(CommandInfo commandInfo)
		{
			IReadOnlyList<ParameterInfo> parms = commandInfo.Parameters;

			StringBuilder format = new StringBuilder();
			int count = parms.Count;
			if (count != 0) format.Append("[");
			int currentCount = 1;
			foreach (ParameterInfo parm in parms)
			{
				format.Append(parm);

				if (currentCount != count) format.Append(", ");
				currentCount += 1;
			}

			if (count != 0) format.Append("]");

			return format.ToString();
		}

		private ModuleInfo GetModule(string moduleName)
		{
			IEnumerable<ModuleInfo> result = from a in _cmdService.Modules
				where a.Name == moduleName
				select a;

			ModuleInfo module = result.FirstOrDefault();
			return module;
		}
	}
}
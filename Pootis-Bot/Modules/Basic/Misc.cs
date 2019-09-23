﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using Pootis_Bot.Services;

namespace Pootis_Bot.Modules.Basic
{
	public class Misc : ModuleBase<SocketCommandContext>
	{
		// Module Information
		// Original Author   - Creepysin
		// Description      - Misc commands
		// Contributors     - Creepysin, 

		private readonly VoteGiveawayService _voteGiveawayService;

		public Misc()
		{
			_voteGiveawayService = new VoteGiveawayService();
		}

		[Command("pick")]
		[Summary("Picks between two things")]
		public async Task PickOne([Remainder] string message)
		{
			string[] options = message.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

			Random r = new Random();
			string selection = options[r.Next(0, options.Length)];
			await Context.Channel.SendMessageAsync("Choice for " + Context.Message.Author.Mention + "\nI Choose: " +
			                                       selection);
		}

		[Command("roll")]
		[Summary("Roles between 0 and 50 or between two custom numbers")]
		public async Task Roll(int min = 0, int max = 50)
		{
			Random r = new Random();
			int random = r.Next(min, max);
			await Context.Channel.SendMessageAsync("The number was: " + random);
		}

		[Command("vote", RunMode = RunMode.Async)]
		[Summary("Starts a vote")]
		public async Task Vote(string time, string title, string description, string yesEmoji, string noEmoji)
		{
			await _voteGiveawayService.StartVote(Context.Guild, Context.Channel, Context.User, time, title, description,
				yesEmoji, noEmoji);
		}

		[Command("reminds")]
		[Summary("Reminds you, duh (In Seconds)")]
		[Alias("res")]
		public async Task Remind(int seconds, [Remainder] string remindmsg)
		{
			await Context.Channel.SendMessageAsync(
				$"Ok, i will send you the message '{remindmsg}' in {seconds} seconds.");
			await ReminderService.RemindAsyncSeconds(Context.User, seconds, remindmsg);
		}
	}
}
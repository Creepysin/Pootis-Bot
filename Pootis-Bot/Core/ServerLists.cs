﻿using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using Pootis_Bot.Entities;

namespace Pootis_Bot.Core
{
	public class ServerLists
	{
		private const string ServerListFile = "Resources/serverlist.json";
		public static List<GlobalServerList> Servers;

		static ServerLists()
		{
			if (DataStorage.SaveExists(ServerListFile))
			{
				Servers = DataStorage.LoadServerList(ServerListFile).ToList();
			}
			else
			{
				Servers = new List<GlobalServerList>();
				SaveServerList();
			}
		}

		/// <summary>
		/// Saves all the servers
		/// </summary>
		public static void SaveServerList()
		{
			DataStorage.SaveServerList(Servers, ServerListFile);
		}

		/// <summary>
		/// Gets a server, or creates one if needed
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public static GlobalServerList GetServer(SocketGuild server)
		{
			return GetOrCreateServer(server.Id);
		}

		private static GlobalServerList GetOrCreateServer(ulong id)
		{
			IEnumerable<GlobalServerList> result = from a in Servers
				where a.ServerId == id
				select a;

			GlobalServerList server = result.FirstOrDefault();
			if (server == null) server = CreateServer(id);
			return server;
		}

		private static GlobalServerList CreateServer(ulong id)
		{
			GlobalServerList newServer = new GlobalServerList
			{
				ServerId = id,
				WelcomeMessageEnabled = false,
				WelcomeChannel = 0,
				WelcomeGoodbyeMessage = "Goodbye [user]. We hope you enjoyed your stay.",
				WelcomeMessage =
					"Hello [user]! Thanks for joining **[server]**. Please check out the rules first then enjoy your stay.",
				RuleEnabled = false,
				RuleRole = null,
				RuleMessageId = 0,
				AntiSpamSettings = new GlobalServerList.AntiSpamSettingsInfo
				{
					RoleToRoleMentionWarnings = 3, MentionUsersPercentage = 45, MentionUserEnabled = true
				}
			};


			Servers.Add(newServer);
			SaveServerList();
			return newServer;
		}
	}
}
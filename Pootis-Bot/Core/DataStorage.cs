﻿using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Pootis_Bot.Entities;

namespace Pootis_Bot.Core
{
	public static class DataStorage
	{
		/// <summary>
		/// Saves all user accounts
		/// </summary>
		/// <param name="accounts">A list of all the user accounts to save</param>
		/// <param name="filePath">Where to save the file</param>
		public static void SaveUserAccounts(IEnumerable<GlobalUserAccount> accounts, string filePath)
		{
			string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
			File.WriteAllText(filePath, json);
		}

		/// <summary>
		/// Gets all the user accounts
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static IEnumerable<GlobalUserAccount> LoadUserAccounts(string filePath)
		{
			if (!File.Exists(filePath)) return null;
			string json = File.ReadAllText(filePath);
			return JsonConvert.DeserializeObject<List<GlobalUserAccount>>(json);
		}

		/// <summary>
		/// Checks if a saved user file exists
		/// </summary>
		/// <param name="filePath">The path to the user save file</param>
		/// <returns></returns>
		public static bool SaveExists(string filePath)
		{
			return File.Exists(filePath);
		}

		/// <summary>
		/// Saves a list of servers
		/// </summary>
		/// <param name="serverLists">A list of servers to save</param>
		/// <param name="filePath">Where to save to</param>
		public static void SaveServerList(IEnumerable<GlobalServerList> serverLists, string filePath)
		{
			string json = JsonConvert.SerializeObject(serverLists, Formatting.Indented);
			File.WriteAllText(filePath, json);
		}

		/// <summary>
		/// Loads a list of all the servers from file
		/// </summary>
		/// <param name="filePath">The path to the server list json file</param>
		/// <returns></returns>
		public static IEnumerable<GlobalServerList> LoadServerList(string filePath)
		{
			if (!File.Exists(filePath)) return null;
			string json = File.ReadAllText(filePath);
			return JsonConvert.DeserializeObject<List<GlobalServerList>>(json);
		}
	}
}
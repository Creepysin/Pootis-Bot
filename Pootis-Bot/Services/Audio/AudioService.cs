﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Pootis_Bot.Core;
using Pootis_Bot.Entities;

namespace Pootis_Bot.Services.Audio
{
    public class AudioService
    {
	    private const string FfmpegLocation = "external/ffmpeg";
	    private const string MusicDir = "Music/";

	    public static readonly List<GlobalServerMusicItem> currentChannels = new List<GlobalServerMusicItem>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target, IMessageChannel channel)
        {
            if (target == null)
            {
                await channel.SendMessageAsync(":musical_note: You need to be in a voice channel");
                return;
            }

            IAudioClient audio = await target.ConnectAsync(); //Connect to the voice channel

            GlobalServerMusicItem item = new GlobalServerMusicItem //Added it to the currentChannels list
            {
                GuildId = guild.Id,
                IsPlaying = false,
                IsExit = false,
                AudioClient = audio,
                AudioChannel = (SocketVoiceChannel)target,
                StartChannel = (ISocketMessageChannel)channel,
            };

            currentChannels.Add(item);
        }

        public async Task LeaveAudio(IGuild guild, IMessageChannel channel)
        {
            if (guild == null) return; //Check if guild is null
            GlobalServerMusicItem serverList = GetMusicList(guild.Id);
            if (serverList == null)
            {
                await channel.SendMessageAsync(":musical_note: Your not in any voice channel!");
                return;
            }

            serverList.IsExit = true;

            if (serverList.FfMpeg != null)
            {
				serverList.FfMpeg.Kill();
				serverList.FfMpeg.Dispose();
            }

            await Task.Delay(100);

            //await serverList.AudioClient.StopAsync(); //Stop the audio client
			serverList.AudioClient.Dispose();

            serverList.IsPlaying = false;

            currentChannels.Remove(GetMusicList(guild.Id)); //Remove it from the currentChannels list
        }

        public async Task StopAudioAsync(IGuild guild, IMessageChannel channel)
        {
            GlobalServerMusicItem serverList = GetMusicList(guild.Id);

            if (serverList == null)
            {
	            await channel.SendMessageAsync(":musical_note: Your not in any voice channel!");
                return;
            }

            if (serverList.IsPlaying == false)
            {
                await channel.SendMessageAsync(":musical_note: No audio is playing.");
            }

            serverList.IsExit = true;
            await channel.SendMessageAsync(":musical_note: Stopping current playing song.");
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, IVoiceChannel target, string search)
        {
            GlobalServerMusicItem serverList = GetMusicList(guild.Id);

            if (serverList == null)
            {
	            await JoinAudio(guild, target, channel);

	            serverList = GetMusicList(guild.Id);
            }

            string fileLoc = SearchAudio(search);   //Search for the song in our current music directory

            if (string.IsNullOrWhiteSpace(fileLoc)) //The search didn't come up with anything, lets attempt to get it from YouTube
            {
                AudioDownload audioDownload = new AudioDownload();
                string result = audioDownload.DownloadAudio(search, channel);
                if (result != null)
                {
                    fileLoc = result;
                }
                else
                    return;
            }

            fileLoc = fileLoc.Replace("\"", "'");

            string tempName = Path.GetFileName(fileLoc);
            string fileName = tempName.Replace(".mp3", "");

            if (serverList.IsPlaying)
            {
	            //Kill and dispose of ffmpeg
                serverList.FfMpeg.Kill();
                serverList.FfMpeg.Dispose();

                await serverList.Discord.FlushAsync();

				//Wait a moment
                await Task.Delay(1000);
            }

            IAudioClient client = serverList.AudioClient;
            Process ffmpeg = serverList.FfMpeg = GetFfmpeg(fileLoc);

            Global.Log($"The song '{fileName}' on server {guild.Name}({guild.Id}) has started.", ConsoleColor.Blue);

            using (Stream output = ffmpeg.StandardOutput.BaseStream) //Start playing the song
            {
                using (serverList.Discord = client.CreatePCMStream(AudioApplication.Music))
                {
                    serverList.IsPlaying = true;
                    bool fail = false;
                    bool exit = false;
                    int bufferSize = 1024;
                    byte[] buffer = new byte[bufferSize];

                    CancellationToken cancellation = new CancellationToken();

                    await channel.SendMessageAsync($":musical_note: Now playing **{fileName}**.");

                    serverList.IsExit = false;

					//To be completely honest, I don't understand much of this.
					//Pootis-Bot and the audio services have always been difficult for me.
					//If anyone could improve upon this I would gladly accept it!
                    while (!fail && !exit)
                    {
                        try
                        {
                            if (cancellation.IsCancellationRequested)
                            {
                                exit = true;
                                break;
                            }

                            if (serverList.IsExit)
                            {
                                exit = true;
                                break;
                            }

                            int read = await output.ReadAsync(buffer, 0, bufferSize, cancellation);
                            if (read == 0)
                            {
                                exit = true;
                                break;
                            }

                            await serverList.Discord.WriteAsync(buffer, 0, read, cancellation);

                            if (serverList.IsPlaying == false)
                            {
                                do
                                {
                                    //Do nothing, wait till is playing is true
                                    await Task.Delay(100, cancellation);
                                } while (serverList.IsPlaying == false);
                            }
                        }
                        catch (OperationCanceledException)
                        {

                        }
                        catch (Exception ex)
                        {
                            await channel.SendMessageAsync($"Sorry an error occured **Error Details**\n{ex.Message}");
                            fail = true;
                        }
                    }

                    //End
                    Global.Log($"The song '{fileName}' on server {guild.Name}({guild.Id}) has stopped.", ConsoleColor.Blue);

                    await serverList.Discord.FlushAsync(cancellation);
                    serverList.Discord.Dispose();
                    serverList.IsPlaying = false;

                    await channel.SendMessageAsync($":musical_note: The song has finished.");

                    //Check to make sure that ffmpeg was disposed
                    ffmpeg.Dispose();
                }
            }
        }

        public async Task PauseAudio(IGuild guild, IMessageChannel channel)
        {
            if (guild == null) return; //Check guild if null

            var musicList = GetMusicList(guild.Id);
            if (musicList == null) return; //Check server list if it is null

            musicList.IsPlaying = !musicList.IsPlaying; //Toggle pause status

            if (musicList.IsPlaying) await channel.SendMessageAsync(":musical_note: Current song has been un-paused");
            else await channel.SendMessageAsync(":musical_note: Current song has been paused");

        }

        public static string SearchAudio(string search)
        {
            if (!Directory.Exists(MusicDir)) Directory.CreateDirectory(MusicDir);

            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(MusicDir);
            FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + search + "*.mp3");

            return filesInDir.Select(foundFile => foundFile.FullName).FirstOrDefault();
        }

        private static Process GetFfmpeg(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = FfmpegLocation,
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        #region List Fuctions

        private GlobalServerMusicItem GetMusicList(ulong guildid)
        {
            var result = from a in currentChannels
                         where a.GuildId == guildid
                         select a;

            var list = result.FirstOrDefault();
            return list;
        }

        #endregion
    }
}

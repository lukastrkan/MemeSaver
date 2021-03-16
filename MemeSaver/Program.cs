using DSharpPlus;
using System;
using System.Threading.Tasks;
using System.IO;
using RestSharp;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace MemeSaver
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            string token = "";
            ulong[] channels;
            using (StreamReader sr = new StreamReader("config.txt"))
            {
                token = await sr.ReadLineAsync();
                string temp = await sr.ReadLineAsync();
                String[] channelsTemp;
                channelsTemp = temp.Split(";");
                channels = Array.ConvertAll(channelsTemp, item => Convert.ToUInt64(item));
            }
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                ReconnectIndefinitely = true,
                AutoReconnect = true
            });

            var client = new RestClient();

            if(!Directory.Exists("memes"))
            {
                Directory.CreateDirectory("memes");
            }
            
            discord.MessageCreated += async(s,e) =>
            {                
                if(channels.Contains((e.Channel.Id)))
                {
                    Console.WriteLine(e.Message.Attachments[0].Url);                    
                    foreach(var attachment in e.Message.Attachments)
                    {
                        using(var client = new WebClient())
                        {                            
                            client.DownloadFile(attachment.Url,Path.Combine("memes", attachment.FileName));
                        }
                    }
                }
                
            };


            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}

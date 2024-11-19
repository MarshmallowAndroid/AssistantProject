using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;
using YoutubeExplode;

namespace Transcriber
{
    internal class CommandManager
    {
        ITranscriber transcriber;

        List<Command> commands = new();

        string wakeCommand;

        public CommandManager(ITranscriber transcriber, string wakeCommand)
        {
            this.transcriber = transcriber;
            this.wakeCommand = wakeCommand;

            commands.Add(new TestCommand(transcriber, Transcriber_Transcribe));

            transcriber.Transcribe += Transcriber_Transcribe;
        }

        private void Transcriber_Transcribe(ITranscriber sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
            string text = eventArgs.Text;
            Console.WriteLine(text);
            foreach (var command in commands)
            {
                if (command.CommandMatch(text))
                {
                    command.Parse(text);
                    break;
                }
            }
            //if (Common.Similarity(text, "Will you never give me up") > 0.5f)
            //{
            //    Console.WriteLine("As you wish.");
            //    PlayYouTube("dQw4w9WgXcQ");
            //}
            //else if (Common.Similarity(text, "goodbye") > 0.5f)
            //{
            //    transcriber?.StopTranscribing();
            //}
            //else if (Common.Similarity(text, "stop playing") > 0.5f)
            //{
            //    outputDevice?.Stop();
            //}
            //else if (Common.Similarity(text, "play on youtube") > 0.5f)
            //{

            //}
        }

        private int FirstIndexFrom(string findFrom, string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                int index = findFrom.IndexOf(strings[i]);
                if (index >= 0) return index;
            }
            return -1;
        }

        static HttpClient httpClient = new();
        static YoutubeClient youtubeClient = new(httpClient);
        static WasapiOut? outputDevice;

        static void PlayYouTube(string videoId)
        {
            StreamClient streamClient = youtubeClient.Videos.Streams;
            StreamManifest streamManifest = streamClient.GetManifestAsync(new VideoId(videoId)).GetAwaiter().GetResult();
            IStreamInfo streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            outputDevice?.Stop();

            outputDevice = new();
            outputDevice.Init(new MediaFoundationReader(streamInfo.Url));
            outputDevice.Play();
        }
    }
}

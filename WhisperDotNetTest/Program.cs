using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;
using System.Diagnostics;
using System.Reflection.Metadata;
using Whisper.net;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Transcriber
{
    internal class Program
    {
        static int audioLengthMs = 10000;
        static ITranscriber? transcriber;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            Console.WriteLine("Initializing Whisper...");
            transcriber = new WhisperDotNetTranscriber(args[0]);
            transcriber.Initialize();

            transcriber.StartTranscribing();
            Console.WriteLine("Begin speaking.");
            //transcriber.Transcribe += Transcriber_Transcribe;
            CommandManager commandManager = new(transcriber, "Hey buddy");
            (transcriber as WhisperDotNetTranscriber)?.ThreadJoin();
        }

        private static void Transcriber_Transcribe(ITranscriber sender, ITranscriber.TranscribeEventArgs e)
        {
            string text = e.Text.ToLower();
            text.Replace("can you ", "");
            text.Replace("may you ", "");
            text.Replace("can you ", "");
            text.Replace("can you ", "");

            Console.WriteLine("Probability: " + e.Probability); // for debugging remove later
            Console.WriteLine(text);

            if (Common.Similarity(text, "Will you never give me up") > 0.5f)
            {
                Console.WriteLine("As you wish.");
                PlayYouTube("dQw4w9WgXcQ");
            }
            else if (Common.Similarity(text, "Kill yourself") > 0.5f)
            {
                transcriber?.StopTranscribing();
            }
            else if (Common.Similarity(text, "play on youtube") > 0.5f)
            {

            }
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

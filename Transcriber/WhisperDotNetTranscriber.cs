using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whisper.net;
using Whisper.net.LibraryLoader;

namespace Transcriber
{
    public class WhisperDotNetTranscriber : ITranscriber
    {
        private const int audioDetectionLengthMs = 2000;
        private const int audioLengthMs = 10000;

        private readonly string model;

        // initialize audio manager with 10 seconds of buffer
        private readonly InputDeviceBuffer audioManager;
        private readonly Thread backgroundThread;

        private bool continueRunning = true;
        private WhisperProcessor? processor;

        public WhisperDotNetTranscriber(string modelPath, IWaveIn inputDevice)
        {
            model = modelPath;
            audioManager = new(audioLengthMs, inputDevice);

            backgroundThread = new(BackgroundThreadStart);
        }

        public event EventHandler<ITranscriber.TranscribeEventArgs>? Transcribe;

        public void Dispose()
        {
            ThreadJoin();
            StopTranscribing();
            processor?.Dispose();
        }

        public void Initialize()
        {
            RuntimeOptions.Instance.SetLoadedLibrary(RuntimeLibrary.Cuda);
            RuntimeOptions.Instance.SetUseFlashAttention(true);

            WhisperFactory whisper = WhisperFactory.FromPath(model);
            processor = whisper.CreateBuilder()
                .WithSegmentEventHandler(OnSegment)
                //.WithBeamSearchSamplingStrategy().ParentBuilder
                .WithProbabilities()
                .WithNoContext()
                .WithLanguageDetection()
                .WithTranslate()
                .Build();
        }

        public void StartTranscribing()
        {
            backgroundThread.Start();
        }

        private void BackgroundThreadStart()
        {
            if (processor is null)
                throw new Exception("Please initialize the transcriber.");

            // stopwatch to keep track of elapsed times
            Stopwatch stopwatch = new();
            stopwatch.Start();

            // store the last recorded time
            double last = stopwatch.Elapsed.TotalMilliseconds;

            // two separate audio streams; one for voice detection and the other for the rest of the speech
            float[] voiceDetectionAudio;
            float[] audio;

            while (continueRunning)
            {
                // get the seconds passed since the last recorded time
                double now = stopwatch.Elapsed.TotalMilliseconds;
                double timeDiff = now - last;

                // we must buffer at least 2 seconds of audio for voice detection
                if (timeDiff < audioDetectionLengthMs)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // get the first two seconds of audio
                voiceDetectionAudio = audioManager.Get(audioDetectionLengthMs);
                if (Common.VoiceActivityDetected(voiceDetectionAudio, 16000, 1000, 0.6f, 100.0f))
                {
                    //Console.WriteLine("Voice activity detected.");
                    audio = audioManager.Get(audioLengthMs);
                }
                else
                {
                    Thread.Sleep(100);
                    continue;
                }

                last = now;

                processor.Process(audio);
                audioManager.Clear();
            }
        }

        public void ThreadJoin()
        {
            backgroundThread.Join();
        }

        private void OnSegment(SegmentData e)
        {
            if (e.Language != "en")
            {

            }

            Transcribe?.Invoke(this,
                new ITranscriber.TranscribeEventArgs(e.Text.Trim(), e.Probability));
        }

        public void StopTranscribing()
        {
            continueRunning = false;
        }
    }
}

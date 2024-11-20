using NAudio.Wave;
using System.Diagnostics;
using Whisper.net;
using Whisper.net.LibraryLoader;
using Whisper.net.Logger;

namespace GooeyWpf.Transcriber
{
    public class WhisperDotNetTranscriber : ITranscriber
    {
        private const int audioDetectionLengthMs = 2000;
        private const int audioLengthMs = 6000;

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

        public event EventHandler? VoiceActivity;

        public event EventHandler? VoiceActivityDone;

        public void Dispose()
        {
            ThreadJoin();
            StopTranscribing();
            processor?.Dispose();
        }

        public void Initialize()
        {
            //RuntimeOptions.Instance.SetUseFlashAttention(true);

            LogProvider.Instance.OnLog += (l, s) =>
            {
                Debug.WriteLine($"[{l}] {s}");
            };

            RuntimeOptions.Instance.SetRuntimeLibraryOrder([RuntimeLibrary.Cuda]);

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
                if (VoiceActivityDetected(voiceDetectionAudio, 16000, audioDetectionLengthMs / 2, 0.5f, 0.0f))
                {
                    audio = audioManager.Get(audioLengthMs);
                    VoiceActivity?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    VoiceActivityDone?.Invoke(this, EventArgs.Empty);
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
            Transcribe?.Invoke(this,
                new ITranscriber.TranscribeEventArgs(e.Text.Trim(), e.Probability, e.Language));
        }

        public void ManualInput(string input)
        {
            Transcribe?.Invoke(this,
                new ITranscriber.TranscribeEventArgs(input, 1.0f, "en"));
        }

        public void StopTranscribing()
        {
            continueRunning = false;
        }

        private static bool VoiceActivityDetected(float[] samples, int sampleRate, int lastMs, float threshold, float frequencyThreshold)
        {
            int sampleCount = samples.Length;
            int sampleCountLast = sampleRate * lastMs / 1000;

            if (sampleCountLast >= sampleCount)
                return false;

            //using WaveFileWriter writer1 = new("vad_debug.wav", new WaveFormat(16000, 1));
            //writer1.WriteSamples(samples, 0, samples.Length);
            //writer1.Flush();

            if (frequencyThreshold > 0.0f)
                HighPassFilter(samples, frequencyThreshold, sampleRate);

            //using WaveFileWriter writer2 = new("vad_debug_hpf.wav", new WaveFormat(16000, 1));
            //writer2.WriteSamples(samples, 0, samples.Length);
            //writer2.Flush();

            float energyAll = 0.0f;
            float energyLast = 0.0f;

            for (int i = 0; i < sampleCount; i++)
            {
                energyAll += MathF.Abs(samples[i]);
                if (i >= sampleCount - sampleCountLast)
                    energyLast += MathF.Abs(samples[i]);
            }

            energyAll /= sampleCount;
            energyLast /= sampleCountLast;

            //Debug.WriteLine($"energyAll: {energyAll}, energyLast: {energyLast}, threshold * energyAll: {threshold * energyAll}");

            if (energyLast > threshold * energyAll)
                return false;

            return true;
        }

        private static void HighPassFilter(float[] data, float cutoff, int sampleRate)
        {
            float rc = 1.0f / (2.0f * MathF.PI * cutoff);
            float dt = 1.0f / sampleRate;
            float alpha = dt / (rc + dt);

            float y = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                y = alpha * (y + data[i] - data[i - 1]);
                data[i] = y;
            }
        }
    }
}
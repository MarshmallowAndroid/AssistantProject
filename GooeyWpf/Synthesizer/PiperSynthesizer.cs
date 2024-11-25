using NAudio.Wave;
using System.Diagnostics;
using System.IO;

namespace GooeyWpf.Synthesizer
{
    public class PiperSynthesizer : ISynthesizer
    {
        private readonly WasapiOut outputDevice = new();
        private readonly ProcessStartInfo processStartInfo;
        private Process? piperProcess;

        public event EventHandler<bool>? LipSync;

        public PiperSynthesizer(string piperExecutable, string model, int speaker, float length)
        {
            processStartInfo = new()
            {
                FileName = piperExecutable,
                Arguments = $"-m {model} -s {speaker} --length_scale {length} --output_raw",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            Initialize();
        }

        private Stream Initialize()
        {
            piperProcess = Process.Start(processStartInfo)!;
            if (piperProcess is null)
                throw new Exception("Unable to start Piper process.");

            RawSourceWaveStream rawSourceWaveStream = new(piperProcess.StandardOutput.BaseStream, new WaveFormat(22050, 1));
            LipsyncSampleProvider sampleProvider = new(rawSourceWaveStream.ToSampleProvider());
            sampleProvider.LipSync += SampleProvider_LipSync;

            outputDevice.Init(sampleProvider);
            outputDevice.Play();

            return piperProcess.StandardOutput.BaseStream;
        }

        private void SampleProvider_LipSync(object? sender, bool e)
        {
            LipSync?.Invoke(sender, e);
        }

        public void Synthesize(string text)
        {
            if (piperProcess?.HasExited ?? true)
            {
                Initialize();
            }

            piperProcess?.StandardInput.WriteLine(text);
        }
    }
}
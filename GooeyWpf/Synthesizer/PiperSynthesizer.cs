using System.Diagnostics;
using System.IO;

namespace GooeyWpf.Synthesizer
{
    public class PiperSynthesizer : ISynthesizer
    {
        private readonly ProcessStartInfo processStartInfo;
        private Process? piperProcess;

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

            OutputStream = Initialize();
        }

        public Stream OutputStream { get; private set; }

        private Stream Initialize()
        {
            piperProcess = Process.Start(processStartInfo)!;
            if (piperProcess is null)
                throw new Exception("Unable to start Piper process.");
            return piperProcess.StandardOutput.BaseStream;
        }

        public void Synthesize(string text)
        {
            if (piperProcess is null)
            {
                OutputStream = Initialize();
            }

            piperProcess?.StandardInput.WriteLine(text);
        }
    }
}
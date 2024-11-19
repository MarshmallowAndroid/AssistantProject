using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Reflection;

namespace Synthesizer
{
    public class PiperSynthesizer : ISynthesizer
    {
        private readonly Process piperProcess;

        public PiperSynthesizer(string piperExecutable, string model, int speaker, float length)
        {
            ProcessStartInfo psi = new()
            {
                FileName = piperExecutable,
                Arguments = $"-m {model} -s {speaker} --length_scale {length} --output_raw",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            piperProcess = Process.Start(psi)!;
            if (piperProcess is null)
                throw new Exception("Unable to start Piper process.");
            OutputStream = piperProcess.StandardOutput.BaseStream;
        }

        public Stream OutputStream { get; }

        public void Synthesize(string text)
        {
            piperProcess.StandardInput.WriteLine(text);
        }
    }
}

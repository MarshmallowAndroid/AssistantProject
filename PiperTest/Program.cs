using NAudio.Wave;
using System.Diagnostics;

namespace PiperTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            string piperExecutable = @"C:\Users\jacob\Downloads\piper_windows_amd64\piper\piper.exe";
            string model = @"C:\Users\jacob\Downloads\piper_windows_amd64\en_GB-semaine-medium.onnx";

            ProcessStartInfo psi = new()
            {
                FileName = piperExecutable,
                Arguments = $"-m {model} -s 3 --length_scale 1.05 --output_raw",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            Process? piperProcess = Process.Start(psi);

            if (piperProcess is null) return;

            RawSourceWaveStream waveStream = new(piperProcess.StandardOutput.BaseStream, new WaveFormat(22050, 1));

            WasapiOut outputDevice = new();
            outputDevice.Init(waveStream);
            outputDevice.Play();

            while (outputDevice.PlaybackState != PlaybackState.Stopped)
            {
                Console.Write("Enter prompt: ");
                string input = Console.ReadLine() ?? "";
                piperProcess.StandardInput.WriteLine(input);
            }
        }
    }
}

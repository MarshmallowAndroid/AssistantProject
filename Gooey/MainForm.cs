using Gooey.Commands;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Synthesizer;
using Transcriber;

namespace Gooey
{
    public partial class MainForm : Form
    {
        WasapiOut outputDevice = new();
        ISynthesizer synthesizer;
        ITranscriber transcriber;
        CommandManager commandManager;

        public MainForm()
        {
            InitializeComponent();
            synthesizer = new PiperSynthesizer(
                @"C:\Users\jacob\Downloads\piper_windows_amd64\piper\piper.exe",
                @"C:\Users\jacob\Downloads\piper_windows_amd64\en_GB-semaine-medium.onnx",
                3, 1.05f);
            RawSourceWaveStream rawSourceWaveStream = new(synthesizer.OutputStream, new WaveFormat(22050, 1));
            LipsyncSampleProvider sampleProvider = new(rawSourceWaveStream.ToSampleProvider());
            sampleProvider.Lipsync += SampleProvider_Lipsync;
            outputDevice.Init(sampleProvider);
            outputDevice.Play();
            MMDeviceEnumerator deviceEnumerator = new();
            WasapiCapture captureDevice = new(deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia), true)
            {
                WaveFormat = new(16000, 1)
            };
            transcriber = new WhisperDotNetTranscriber(
                @"C:\Users\jacob\Desktop\ggml-medium.bin",
                captureDevice);
            transcriber.Initialize();
            transcriber.StartTranscribing();
            commandManager = new(transcriber, "");
            commandManager.RegisterCommand(new KillCommand(synthesizer, responseLabel, commandManager, transcriber));
            commandManager.RegisterCommand(new TestCommand(synthesizer, responseLabel, transcriber));
            commandManager.Transcribe += CommandManager_Transcribe;
        }

        private void SampleProvider_Lipsync(bool mouthOpen)
        {
            if (mouthOpen)
            {
                pictureBox1.OpenMouth();
            }
            else
            {
                pictureBox1.CloseMouth();
            }
        }

        private void CommandManager_Transcribe(ITranscriber sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
            Invoke(() =>
            {
                label2.Text = eventArgs.Text;
            });
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            synthesizer.Synthesize("i am loud and obnoxious");
            synthesizer.Synthesize("i like music that rhymes");
            synthesizer.Synthesize("i'm a fraction of the population but commit half the crimes.");
            Thread.Sleep(500);
            synthesizer.Synthesize("who am i");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            transcriber.StopTranscribing();
            commandManager.Stop();
        }
    }
}

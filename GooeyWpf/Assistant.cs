using GooeyWpf.Commands;
using GooeyWpf.Synthesizer;
using GooeyWpf.Transcriber;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GooeyWpf
{
    internal class Assistant : Singleton<Assistant>, IDisposable
    {
        public const string AssistantName = "Britney";
        public static readonly string[] WakeWordVariations =
        [
            "Hey Britney", "Hey Brittany", "Hey Brittney", "Hey Britany", "Hey Britny", "Hey Brit Knee",
            "Britney", "Brittany", "Brittney", "Britany", "Brit Knee"
        ];

        private readonly WasapiOut outputDevice = new();
        private readonly ISynthesizer synthesizer;
        private readonly WhisperDotNetTranscriber transcriber;
        private readonly CommandManager commandManager;

        private DirectCommand? directCommand;

        public Assistant()
        {
            string[] piperVoices = Directory.GetFiles("PiperVoice", "*.onnx");
            string[] whisperCppModels = Directory.GetFiles("WhisperCppModel", "*.bin");

            synthesizer = new PiperSynthesizer(
                @"Piper\piper.exe",
                piperVoices[0],
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
            transcriber = new WhisperDotNetTranscriber(whisperCppModels[0], captureDevice);
            transcriber.Initialize();
            transcriber.StartTranscribing();
            commandManager = new(transcriber, "hey " + AssistantName, WakeWordVariations);
            commandManager.Wake += CommandManager_Wake;

            transcriber.VoiceActivity += Assistant_VoiceActivity;
            transcriber.VoiceActivityDone += Assistant_VoiceActivityDone;
        }

        private void Assistant_VoiceActivityDone(object? sender, EventArgs e)
        {
            VoiceActivityDone?.Invoke(sender, e);
        }

        private void Assistant_VoiceActivity(object? sender, EventArgs e)
        {
            VoiceActivity?.Invoke(sender, e);
        }

        public delegate void OnLipSync(bool mouthOpen);

        public event OnLipSync? LipSync;

        public event EventHandler? VoiceActivity;

        public event EventHandler? VoiceActivityDone;

        private void SampleProvider_Lipsync(bool mouthOpen)
        {
            LipSync?.Invoke(mouthOpen);
        }

        private void CommandManager_Wake(object? sender, EventArgs e)
        {
            directCommand?.DirectResponse(
            [
                "What can I do for you?",
                "What's up?"
            ]);
        }

        public CommandManager CommandManager => commandManager;

        public ITranscriber Transcriber => transcriber;

        public void BindCommands(ListBox chatLog, AvatarController avatarController)
        {
            directCommand = new DirectCommand(transcriber, synthesizer, chatLog, avatarController);

            IEnumerable<TypeInfo> commandTypes = Assembly.GetExecutingAssembly().DefinedTypes
                .Where(t => t.CustomAttributes.Any(t => t.AttributeType == typeof(CommandAttribute)));

            IEnumerable<Command> commands = commandTypes.Select(ct =>
            {
                return (Command)Activator.CreateInstance(ct, [transcriber, synthesizer, chatLog, avatarController])!;
            });

            commandManager.RegisterCommand(new KillCommand(transcriber, synthesizer, chatLog, avatarController, commandManager));
            commandManager.RegisterCommands(commands);

            IEnumerable<TypeInfo> smalltalkTypes = Assembly.GetExecutingAssembly().DefinedTypes
                .Where(t => t.CustomAttributes.Any(t => t.AttributeType == typeof(SmallTalkCommandAttribute)));

            IEnumerable<Command> smalltalkCommands = smalltalkTypes.Select(ct =>
            {
                return (Command)Activator.CreateInstance(ct, [transcriber, synthesizer, chatLog, avatarController])!;
            });

            commandManager.RegisterSmalltalkCommands(smalltalkCommands);
        }

        public void ManualInput(string text)
        {
            transcriber.ManualInput(text);
        }

        public void Dispose()
        {
            transcriber.StopTranscribing();
            commandManager.Stop();
        }
    }
}
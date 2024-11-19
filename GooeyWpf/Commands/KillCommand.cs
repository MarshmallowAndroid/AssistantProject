using GooeyWpf.Services.Synthesizer;
using GooeyWpf.Services.Transcriber;
using System.Windows;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    internal class KillCommand : InteractiveCommand
    {
        private readonly CommandManager commandManager;

        public KillCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog,
            AvatarController avatarController, CommandManager commandManager) : base(transcriber, synthesizer, chatLog, avatarController)
        {
            this.commandManager = commandManager;
        }

        public override bool CommandMatch(string text)
        {
            return Common.Similarity(text, "kill yourself") > 0.85f ||
                Common.Similarity(text, "goodbye") > 0.6f ||
                Common.Similarity(text, "your work here is done") > 0.6f;
        }

        public override void Parse(string text)
        {
            ChangeExpression(Expression.Frown);
            Respond([
                "If you say so.",
                "As you wish.",
                "Goodbye!"
                ]);
            //Process.Start("shutdown.exe", "/l /t 0");
            Task.Delay(3000).ContinueWith(t =>
            {
                transcriber.StopTranscribing();
                commandManager.Stop();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            });
        }

        protected override void OnCommandTranscribe(object? sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
        }
    }
}
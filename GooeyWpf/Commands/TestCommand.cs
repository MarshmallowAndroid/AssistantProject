using GooeyWpf.Synthesizer;
using GooeyWpf.Transcriber;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    [Command]
    internal class TestCommand : InteractiveCommand
    {
        public TestCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.Similarity(text, "ask me if i would like a burger") > 0.8f;
        }

        public override void Parse(string text)
        {
            Respond("would you like a burger?");
            Enter();
        }

        protected override void OnCommandTranscribe(object? sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
            string text = eventArgs.Text;

            if (Common.IsPositive(text))
            {
                Respond("cool");
                Exit();
            }
            else if (Common.IsNegative(text))
            {
                Respond("alright then");
                Exit();
            }

            base.OnCommandTranscribe(sender, eventArgs);
        }
    }
}
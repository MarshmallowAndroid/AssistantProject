using GooeyWpf.Synthesizer;
using GooeyWpf.Transcriber;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    internal class SayCommand : InteractiveCommand
    {
        public SayCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            string trimmed = text.ToLower().Trim();
            List<string> split = trimmed.Split(' ').ToList();
            split.IndexOf("say");
            return false;
        }

        public override void Parse(string text)
        {
        }
    }
}
using GooeyWpf.Services.Synthesizer;
using GooeyWpf.Services.Transcriber;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    [Command]
    internal class EmotionsCommand : InteractiveCommand
    {
        public EmotionsCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.Similarity("can you feel emotions", text) > 0.9f;
        }

        public override void Parse(string text)
        {
            Respond(
                [
                "I'm sorry, I'm just software and cannot feel emotions. However, I do specialize in speech recognition. I can understand what you're saying",
                "No. At least I don't think I do."
                ]);
        }
    }
}
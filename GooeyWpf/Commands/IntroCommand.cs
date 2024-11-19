using GooeyWpf.Services.Synthesizer;
using GooeyWpf.Services.Transcriber;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    [Command]
    internal class IntroCommand : InteractiveCommand
    {
        public IntroCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.SimilarOrMatch(text, [
                "who are you", "where did you come from", "who are you and where did you come from"
             ], 0.6f);
        }

        public override void Parse(string text)
        {
            Respond(
                ["I was made in iAcademy. My creators are JMJK; Jilliana, Marc, Jacob, and Kyle. They are students taking up bachelors in software engineering. I am one of their creations."],
                ["I was made in eye-academy. My creators are JMJK; Jilliana, Marc, Jacob, and Kyle. They are students taking up bachelors in software engineering. I am one of their creations."]);
        }
    }
}
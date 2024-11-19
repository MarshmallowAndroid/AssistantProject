using GooeyWpf.Services.Synthesizer;
using GooeyWpf.Services.Transcriber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    class EyeAcademyDirectory : InteractiveCommand
    {
        public EyeAcademyDirectory(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return (text.Contains("can i ask") || text.Contains("ask about")) &&
                (text.Contains("iacademy"));
        }

        public override void Parse(string text)
        {
            Enter();
        }
    }
}

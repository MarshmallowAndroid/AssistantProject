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
    [Command]
    internal class BarrelRollCommand : InteractiveCommand
    {
        public BarrelRollCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.RemovePunctuation(text).Contains("do a barrel roll");
        }

        public override void Parse(string text)
        {
            Respond(["Sure.", "Doing a barrel roll."]);
            avatarController.BarrelRoll();
        }
    }
}

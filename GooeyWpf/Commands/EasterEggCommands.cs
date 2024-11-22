using GooeyWpf.Services;
using GooeyWpf.Synthesizer;
using GooeyWpf.Transcriber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    [SmallTalkCommand]
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

    [SmallTalkCommand]
    internal class PodBayDoorsCommand : InteractiveCommand
    {
        public PodBayDoorsCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.Similarity(Common.RemovePunctuation(text), "open the pod bay doors") > 0.7f;
        }

        public override void Parse(string text)
        {
            ChangeExpression(Expression.Hal);
            Respond("I'm sorry, Dave. I'm afraid I can't do that.");
        }
    }

    [SmallTalkCommand]
    internal class OopsCommand : InteractiveCommand
    {
        public OopsCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.RemovePunctuation(text).Contains("oops i did it again");
        }

        public override void Parse(string text)
        {
            MusicService.Instance.PlayMp3(Application.GetResourceStream(Common.Resource("/Sounds/BritneyOops.mp3")).Stream);
        }
    }
}

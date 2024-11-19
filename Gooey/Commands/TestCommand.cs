using Synthesizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transcriber;

namespace Gooey.Commands
{
    internal class TestCommand : InteractiveCommand
    {
        public TestCommand(ISynthesizer synthesizer, Control outputText, ITranscriber transcriber) : base(synthesizer, outputText, transcriber)
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

        protected override void OnCommandTranscribe(ITranscriber sender, ITranscriber.TranscribeEventArgs eventArgs)
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

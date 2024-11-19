using Synthesizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transcriber;

namespace Gooey.Commands
{
    internal class TimeCommand : InteractiveCommand
    {
        public TimeCommand(ISynthesizer synthesizer, Control outputText, ITranscriber transcriber) : base(synthesizer, outputText, transcriber)
        {
        }

        public override bool CommandMatch(string text)
        {
            throw new NotImplementedException();
        }

        public override void Parse(string text)
        {
            throw new NotImplementedException();
        }
    }
}

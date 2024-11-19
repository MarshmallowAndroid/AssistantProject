using Synthesizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transcriber;

namespace Gooey.Commands
{
    internal class SayCommand : InteractiveCommand
    {
        public SayCommand(ISynthesizer synthesizer, Control outputText, ITranscriber transcriber) : base(synthesizer, outputText, transcriber)
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

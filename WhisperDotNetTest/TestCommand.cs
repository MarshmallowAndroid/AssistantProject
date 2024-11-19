using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcriber
{
    internal class TestCommand : Command
    {
        public TestCommand(ITranscriber transcriber, ITranscriber.OnTranscribe originalTranscribeEvent) : base(transcriber, originalTranscribeEvent)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.Similarity(text, "ask me if i would like a burger") > 0.5f;
        }

        public override void Parse(string text)
        {
            Console.WriteLine("would you like a burger?");
            Enter();
        }

        protected override void OnCommandTranscribe(ITranscriber sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
            string text = eventArgs.Text;

            if (Common.IsPositive(text))
            {
                Console.WriteLine("awesome");
                Exit();
            }
            else if (Common.IsNegative(text))
            {
                Console.WriteLine("alright then");
                Exit();
            }
        }
    }
}

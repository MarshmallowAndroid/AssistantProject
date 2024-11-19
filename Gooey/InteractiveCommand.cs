using Synthesizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transcriber;

namespace Gooey
{
    internal abstract class InteractiveCommand : Command
    {
        protected readonly ISynthesizer synthesizer;
        protected readonly Control outputText;

        protected InteractiveCommand(ISynthesizer synthesizer, Control outputText, ITranscriber transcriber) : base(transcriber)
        {
            this.synthesizer = synthesizer;
            this.outputText = outputText;
        }

        protected override void OnCommandTranscribe(object? sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
            outputText.Invoke(() =>
            {
                outputText.Text = eventArgs.Text;
            });
        }

        protected void Respond(string text)
        {
            outputText.Invoke(() =>
            {
                outputText.Text = text;
            });
            synthesizer.Synthesize(text);
        }
    }
}

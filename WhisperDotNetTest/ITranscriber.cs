using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcriber
{
    internal interface ITranscriber : IDisposable
    {
        public record TranscribeEventArgs(string Text, float Probability);
        delegate void OnTranscribe(ITranscriber sender, TranscribeEventArgs eventArgs);
        event OnTranscribe? Transcribe;

        void Initialize();
        void StartTranscribing();
        void StopTranscribing();
    }
}

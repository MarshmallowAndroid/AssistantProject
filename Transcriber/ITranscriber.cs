using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcriber
{
    public interface ITranscriber : IDisposable
    {
        public record TranscribeEventArgs(string Text, float Probability);
        event EventHandler<TranscribeEventArgs>? Transcribe;

        void Initialize();
        void StartTranscribing();
        void StopTranscribing();
    }
}

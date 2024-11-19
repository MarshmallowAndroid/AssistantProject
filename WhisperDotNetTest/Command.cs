using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcriber
{
    internal abstract class Command(ITranscriber transcriber, ITranscriber.OnTranscribe originalTranscribeEvent)
    {
        public abstract bool CommandMatch(string text);

        public abstract void Parse(string text);

        protected abstract void OnCommandTranscribe(ITranscriber sender, ITranscriber.TranscribeEventArgs eventArgs);

        protected virtual void Enter()
        {
            transcriber.Transcribe -= originalTranscribeEvent;
            transcriber.Transcribe += OnCommandTranscribe;
        }

        protected virtual void Exit()
        {
            transcriber.Transcribe -= OnCommandTranscribe;
            transcriber.Transcribe += originalTranscribeEvent;
        }
    }
}

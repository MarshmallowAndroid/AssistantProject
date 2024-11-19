using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcriber
{
    public abstract class Command
    {
        protected readonly ITranscriber transcriber;

        public Command(ITranscriber transcriber)
        {
            this.transcriber = transcriber;
        }

        public EventHandler<ITranscriber.TranscribeEventArgs>? OriginalTranscribeEvent { get; set; }

        public abstract bool CommandMatch(string text);

        public abstract void Parse(string text);

        protected abstract void OnCommandTranscribe(object? sender, ITranscriber.TranscribeEventArgs eventArgs);

        protected void Enter()
        {
            transcriber.Transcribe -= OriginalTranscribeEvent;
            transcriber.Transcribe += OnCommandTranscribe;
        }

        protected void Exit()
        {
            transcriber.Transcribe -= OnCommandTranscribe;
            transcriber.Transcribe += OriginalTranscribeEvent;
        }
    }
}

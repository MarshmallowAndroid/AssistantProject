using GooeyWpf.Transcriber;

namespace GooeyWpf.Commands
{
    public abstract class Command(ITranscriber transcriber)
    {
        private readonly Stack<EventHandler<ITranscriber.TranscribeEventArgs>> enterEventStack = new();
        private readonly Stack<EventHandler<ITranscriber.TranscribeEventArgs>> exitEventStack = new();
        private EventHandler<ITranscriber.TranscribeEventArgs>? lastEvent;
        private EventHandler<ITranscriber.TranscribeEventArgs>? originalTranscribeEvent;
        protected readonly ITranscriber transcriber = transcriber;

        public EventHandler<ITranscriber.TranscribeEventArgs>? OriginalTranscribeEvent
        {
            get => originalTranscribeEvent;
            set
            {
                originalTranscribeEvent = value;
                if (value is not null)
                    lastEvent = value;
            }
        }

        public abstract bool CommandMatch(string text);

        public abstract void Parse(string text);

        protected abstract void OnCommandTranscribe(object? sender, ITranscriber.TranscribeEventArgs eventArgs);

        protected void Enter()
        {
            transcriber.Transcribe -= OriginalTranscribeEvent;
            transcriber.Transcribe += OnCommandTranscribe;
        }

        protected void Enter(EventHandler<ITranscriber.TranscribeEventArgs> transcribeEvent)
        {
            transcriber.Transcribe -= lastEvent;
            transcriber.Transcribe += transcribeEvent;

            if (lastEvent is not null)
                enterEventStack.Push(lastEvent);
            exitEventStack.Push(transcribeEvent);

            lastEvent = transcribeEvent;
        }

        protected void Exit()
        {
            //transcriber.Transcribe -= OnCommandTranscribe;
            //transcriber.Transcribe += OriginalTranscribeEvent;

            if (enterEventStack.Count > 0)
            {
                transcriber.Transcribe -= exitEventStack.Pop();
                transcriber.Transcribe += enterEventStack.Pop();
            }
        }

        protected void Exit(EventHandler<ITranscriber.TranscribeEventArgs> transcribeEvent)
        {
            transcriber.Transcribe -= transcribeEvent;
            transcriber.Transcribe += OriginalTranscribeEvent;
        }
    }
}
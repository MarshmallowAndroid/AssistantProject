namespace GooeyWpf.Transcriber
{
    public interface ITranscriber : IDisposable
    {
        public record TranscribeEventArgs(string Text, float Probability, string Language);

        event EventHandler<TranscribeEventArgs>? Transcribe;
        
        public event EventHandler? VoiceActivity;

        public event EventHandler? VoiceActivityDone;

        void Initialize();

        void StartTranscribing();

        void StopTranscribing();

        void ManualInput(string input);
    }
}
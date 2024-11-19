namespace GooeyWpf.Services.Transcriber
{
    public interface ITranscriber : IDisposable
    {
        public record TranscribeEventArgs(string Text, float Probability, string Language);

        event EventHandler<TranscribeEventArgs>? Transcribe;

        void Initialize();

        void StartTranscribing();

        void StopTranscribing();
    }
}
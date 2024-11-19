using System.IO;

namespace GooeyWpf.Services.Synthesizer
{
    public interface ISynthesizer
    {
        void Synthesize(string text);

        Stream OutputStream { get; }
    }
}
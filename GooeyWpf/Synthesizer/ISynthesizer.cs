using System.IO;

namespace GooeyWpf.Synthesizer
{
    public interface ISynthesizer
    {
        void Synthesize(string text);

        Stream OutputStream { get; }
    }
}
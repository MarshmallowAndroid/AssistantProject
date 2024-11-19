using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    public interface ISynthesizer
    {
        void Synthesize(string text);

        Stream OutputStream { get; }
    }
}

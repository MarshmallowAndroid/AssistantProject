using NAudio.Wave;

namespace GooeyWpf
{
    internal class LipsyncSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;

        public LipsyncSampleProvider(ISampleProvider source)
        {
            this.source = source;
        }

        public WaveFormat WaveFormat => source.WaveFormat;

        public float Threshold { get; set; } = 0.1f;

        public int Read(float[] buffer, int offset, int count)
        {
            float sum = 0f;
            for (int i = offset; i < count; i++)
            {
                sum += MathF.Abs(buffer[i]);
            }
            sum /= count;
            if (sum > Threshold)
                LipSync?.Invoke(this, true);
            else
                LipSync?.Invoke(this, false);
            int read = source.Read(buffer, offset, count);
            if (read < count)
                LipSync?.Invoke(this, false);
            return read;
        }

        public event EventHandler<bool>? LipSync;
    }
}
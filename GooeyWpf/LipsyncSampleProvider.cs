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
                Lipsync?.Invoke(true);
            else
                Lipsync?.Invoke(false);
            int read = source.Read(buffer, offset, count);
            if (read < count)
                Lipsync?.Invoke(false);
            return read;
        }

        public delegate void OnLipsync(bool mouthOpen);

        public event OnLipsync? Lipsync;
    }
}
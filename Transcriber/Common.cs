using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcriber
{
    public class Common
    {
        public static readonly string[] PositiveStrings =
        {
            "yes", "yeah", "yup", "sure", "okay", "uh huh", "alright", "go", "go for it", "do it", "do", "mm hmm"
        };

        public static readonly string[] NegativeStrings =
        {
            "no", "nah", "nope", "don't", "stop", "nuh uh", "uh uh", "cancel"
        };

        public static string RemovePunctuation(string text) =>
            text.Replace(".", "")
                .Replace(",", "")
                .Replace("-", "")
                .Replace(";", "")
                .Replace("'", "")
                .Replace("!", "")
                .Replace("?", "");

        public static bool VoiceActivityDetected(float[] samples, int sampleRate, int lastMs, float threshold, float frequencyThreshold)
        {
            int sampleCount = samples.Length;
            int sampleCountLast = (sampleRate * lastMs) / 1000;

            if (sampleCountLast >= sampleCount)
                return false;

            //using WaveFileWriter writer1 = new("poop.wav", new WaveFormat(16000, 1));
            //writer1.WriteSamples(samples, 0, samples.Length);
            //writer1.Flush();

            if (frequencyThreshold > 0.0f)
                HighPassFilter(samples, frequencyThreshold, sampleRate);

            //using WaveFileWriter writer2 = new("poop_hpf.wav", new WaveFormat(16000, 1));
            //writer2.WriteSamples(samples, 0, samples.Length);
            //writer2.Flush();

            float energyAll = 0.0f;
            float energyLast = 0.0f;

            for (int i = 0; i < sampleCount; i++)
            {
                energyAll += MathF.Abs(samples[i]);
                if (i >= sampleCount - sampleCountLast)
                    energyLast += MathF.Abs(samples[i]);
            }

            energyAll /= sampleCount;
            energyLast /= sampleCountLast;

            //Console.WriteLine($"energyAll: {energyAll}, energylast: {energyLast}, threshold * energyAll: {threshold * energyAll}");

            if (energyLast > threshold * energyAll)
                return false;

            return true;
        }

        public static void HighPassFilter(float[] data, float cutoff, int sampleRate)
        {
            float rc = 1.0f / (2.0f * MathF.PI * cutoff);
            float dt = 1.0f / sampleRate;
            float alpha = dt / (rc + dt);

            float y = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                y = alpha * (y + data[i] - data[i - 1]);
                data[i] = y;
            }
        }

        public static bool IsPositive(string text)
        {
            return SimilarOrMatch(text, PositiveStrings);
        }

        public static bool IsNegative(string text)
        {
            return SimilarOrMatch(text, NegativeStrings);
        }

        public static bool SimilarOrMatch(string text, string[] strings)
        {
            text = text.ToLower();
            bool ret = Similarities(text, strings) > 0.5f;
            if (!ret)
            {
                for (int i = 0; i < strings.Length; i++)
                {
                    if (text.StartsWith(strings[i]) || text.EndsWith(strings[i]))
                        return true;
                }
            }
            return ret;
        }

        public static float Similarities(string s0, string[] s1)
        {
            float largest = 0f;
            for (int i = 0; i < s1.Length; i++)
            {
                float current = Similarity(s0, s1[i]);
                if (current > largest)
                    largest = current;
            }
            return largest;
        }

        public static float Similarity(string s0, string s1)
        {
            int len0 = s0.Length + 1;
            int len1 = s1.Length + 1;

            int[] col = new int[len1];
            int[] prevCol = new int[len1];

            for (int i = 0; i < len1; i++)
            {
                prevCol[i] = i;
            }

            for (int i = 0; i < len0; i++)
            {
                col[0] = i;
                for (int j = 1; j < len1; j++)
                {
                    col[j] = Min(Min(1 + col[j - 1], 1 + prevCol[j]), prevCol[j - 1] + (i > 0 && s0[i - 1] == s1[j - 1] ? 0 : 1));
                }

                (prevCol, col) = (col, prevCol);
            }

            float dist = prevCol[len1 - 1];

            return 1.0f - (dist / Max(s0.Length, s1.Length));
        }

        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }
    }
}

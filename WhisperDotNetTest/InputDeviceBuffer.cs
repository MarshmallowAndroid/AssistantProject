using NAudio.CoreAudioApi;
using NAudio.Wasapi.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcriber
{
    internal class InputDeviceBuffer
    {
        private readonly int lengthMs;

        private readonly WaveFormat waveFormat;
        private readonly WasapiCapture captureDevice;

        private readonly int sampleRate;

        private readonly float[] buffer;
        private int bufferPosition;
        private int bufferLength;

        private readonly object lockObject = new();

        public InputDeviceBuffer(int ms)
        {
            lengthMs = ms;

            MMDeviceEnumerator deviceEnumerator = new();
            captureDevice = new(deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia), true)
            {
                WaveFormat = waveFormat = new(16000, 1)
            };

            captureDevice.StartRecording();
            captureDevice.DataAvailable += CaptureDevice_DataAvailable;

            sampleRate = captureDevice.WaveFormat.SampleRate;

            buffer = new float[sampleRate * lengthMs / 1000];
        }

        private void CaptureDevice_DataAvailable(object? sender, WaveInEventArgs e)
        {
            int sampleCount = e.BytesRecorded / sizeof(short);

            int dataBufferIndex = 0;
            if (sampleCount > buffer.Length)
            {
                sampleCount = buffer.Length;
                dataBufferIndex += e.BytesRecorded - (sampleCount * sizeof(short));
            }

            lock (lockObject)
            {
                if (bufferPosition + sampleCount > buffer.Length)
                {
                    int n0 = buffer.Length - bufferPosition;

                    Copy(e.Buffer, dataBufferIndex, buffer, bufferPosition, n0);
                    Copy(e.Buffer, dataBufferIndex + n0 * sizeof(short), buffer, 0, sampleCount - n0);

                    bufferPosition = (bufferPosition + sampleCount) % buffer.Length;
                    bufferLength = buffer.Length;
                }
                else
                {
                    Copy(e.Buffer, dataBufferIndex, buffer, bufferPosition, sampleCount);
                    bufferPosition = (bufferPosition + sampleCount) % buffer.Length;
                    bufferLength = Common.Min(bufferLength + sampleCount, buffer.Length);
                }
            }
        }

        public float[] Get(int ms)
        {
            lock (lockObject)
            {
                int sampleCount = sampleRate * ms / 1000;
                if (sampleCount > bufferLength)
                {
                    sampleCount = bufferLength;
                }

                float[] result = new float[sampleCount];

                int s0 = bufferPosition - sampleCount;
                if (s0 < 0)
                {
                    s0 += buffer.Length;
                }

                if (s0 + sampleCount > buffer.Length)
                {
                    int n0 = buffer.Length - s0;
                    Array.Copy(buffer, s0, result, 0, n0);
                    Array.Copy(buffer, 0, result, n0, sampleCount - n0);
                }
                else
                {
                    Array.Copy(buffer, s0, result, 0, sampleCount);
                }

                return result;
            }
        }

        public void Clear()
        {
            lock (lockObject)
            {
                bufferPosition = 0;
                bufferLength = 0;
            }
        }

        private static void Copy(byte[] source, int sourceIndex, float[] destination, int destinationIndex, int length)
        {
            for (int i = destinationIndex; i < destinationIndex + length; i++)
            {
                destination[i] = BitConverter.ToInt16(source, sourceIndex) / 32767f;
                sourceIndex += sizeof(short);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcriber
{
    public class CircularSampleBuffer
    {
        private readonly float[] buffer;
        private readonly object lockObject;
        private int writePosition;
        private int readPosition;
        private int sampleCount;

        public int MaxLength => buffer.Length;

        public int Count
        {
            get
            {
                lock (lockObject)
                {
                    return sampleCount;
                }
            }
        }

        public CircularSampleBuffer(int size)
        {
            buffer = new float[size];
            lockObject = new object();
        }

        public int Write(float[] data, int offset, int count)
        {
            lock (lockObject)
            {
                int num = 0;
                if (count > buffer.Length - sampleCount)
                {
                    count = buffer.Length - sampleCount;
                }

                int num2 = Math.Min(buffer.Length - writePosition, count);
                Array.Copy(data, offset, buffer, writePosition, num2);
                writePosition += num2;
                writePosition %= buffer.Length;
                num += num2;
                if (num < count)
                {
                    Array.Copy(data, offset + num, buffer, writePosition, count - num);
                    writePosition += count - num;
                    num = count;
                }

                sampleCount += num;
                return num;
            }
        }

        public int Read(float[] data, int offset, int count)
        {
            lock (lockObject)
            {
                if (count > sampleCount)
                {
                    count = sampleCount;
                }

                int num = 0;
                int num2 = Math.Min(buffer.Length - readPosition, count);
                Array.Copy(buffer, readPosition, data, offset, num2);
                num += num2;
                readPosition += num2;
                readPosition %= buffer.Length;
                if (num < count)
                {
                    Array.Copy(buffer, readPosition, data, offset + num, count - num);
                    readPosition += count - num;
                    num = count;
                }

                sampleCount -= num;
                return num;
            }
        }

        public void Reset()
        {
            lock (lockObject)
            {
                ResetInner();
            }
        }

        private void ResetInner()
        {
            sampleCount = 0;
            readPosition = 0;
            writePosition = 0;
        }

        public void Advance(int count)
        {
            lock (lockObject)
            {
                if (count >= sampleCount)
                {
                    ResetInner();
                    return;
                }

                sampleCount -= count;
                readPosition += count;
                readPosition %= MaxLength;
            }
        }
    }
}

using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooeyWpf.Services
{
    internal class AudioService : Singleton<AudioService>
    {
        private WasapiOut? wasapiOut;

        public AudioService()
        {
        }

        public void Initialize(IWaveProvider waveProvider)
        {
            if (wasapiOut?.PlaybackState != PlaybackState.Stopped)
            {
                Stop();
            }

            wasapiOut = new();
            wasapiOut.Init(waveProvider);
        }

        public void Play() => wasapiOut?.Play();

        public void Stop()
        {
            wasapiOut?.Stop();
            wasapiOut?.Dispose();
        }
    }
}

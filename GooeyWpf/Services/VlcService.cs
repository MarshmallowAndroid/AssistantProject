using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooeyWpf.Services
{
    internal class VlcService : Singleton<VlcService>
    {
        private LibVLC libVlc;

        public VlcService()
        {
            libVlc = new LibVLC();
        }

        public MediaPlayer GetMediaPlayer()
        {
            return new MediaPlayer(libVlc);
        }

        public Media GetMedia(Uri uri)
        {
            return new Media(libVlc, uri);
        }
    }
}

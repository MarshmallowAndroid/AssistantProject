using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;
using YoutubeExplode.Search;

namespace GooeyWpf.Services
{
    internal class YouTubeService : Singleton<YouTubeService>
    {
        private YoutubeClient youtubeClient;

        public YouTubeService()
        {
            youtubeClient = new(HttpClientService.Instance.Client);
        }

        public (IStreamInfo audio, IStreamInfo? video) GetStream(string videoId)
        {
            StreamClient streamClient = youtubeClient.Videos.Streams;
            StreamManifest streamManifest = streamClient.GetManifestAsync(new VideoId(videoId)).Result;

            IEnumerable<VideoOnlyStreamInfo> videos = streamManifest.GetVideoOnlyStreams();

            return (streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate(),
                streamManifest.GetVideoOnlyStreams().FirstOrDefault(
                    v => v.VideoQuality.MaxHeight <= 480, streamManifest.GetVideoOnlyStreams().First()));
        }

        public IAsyncEnumerable<VideoSearchResult> Search(string query)
        {
            return youtubeClient.Search.GetVideosAsync(query);
        }
    }
}

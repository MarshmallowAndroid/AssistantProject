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

        public async Task<(IStreamInfo audio, IStreamInfo? video)> GetStream(string videoId)
        {
            StreamClient streamClient = youtubeClient.Videos.Streams;
            StreamManifest streamManifest = await streamClient.GetManifestAsync(new VideoId(videoId));

            IEnumerable<VideoOnlyStreamInfo> videos = streamManifest.GetVideoOnlyStreams();

            return (streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate(),
                streamManifest.GetVideoOnlyStreams().FirstOrDefault());
        }

        public IAsyncEnumerable<VideoSearchResult> Search(string query)
        {
            return youtubeClient.Search.GetVideosAsync(query);
        }
    }
}

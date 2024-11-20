using GooeyWpf.Services;
using GooeyWpf.Synthesizer;
using GooeyWpf.Transcriber;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using TagLib.Ape;

namespace GooeyWpf.Commands
{
    [Command]
    internal class MusicCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : InteractiveCommand(transcriber, synthesizer, chatLog, avatarController)
    {
        public override bool CommandMatch(string text)
        {
            return text.StartsWith("play ") || text.StartsWith("stop ") ||
                text.Contains("refresh my library") ||
                text.Contains("update my library") ||
                text.Contains("got new music");
        }

        public override void Parse(string text)
        {
            string remaining = Common.RemovePunctuation(text[text.IndexOf(' ')..].Trim()).ToLower();

            if (text.Contains("refresh my library") ||
                text.Contains("update my library") ||
                text.Contains("got new music"))
            {
                Respond([
                    "Alright. I'll update your music library.",
                    "Sure. I'll update your music library."
                    ]);
                MusicService.Instance.UpdateLibrary();
                return;
            }

            if (text.StartsWith("stop"))
            {
                MusicService.Instance.Stop();
                Respond(
                [
                    "Alright. Stopping music.",
                    "Sure."
                ]);
                return;
            }

            if (remaining.EndsWith("a random song")
                || remaining.EndsWith("a song")
                || remaining.EndsWith("something from my library")
                || remaining.EndsWith("some music"))
            {
                MusicService.Instance.PlayRandom();
                RespondSuccess("music");
                return;
            }
            else if (remaining.Split("by").Length > 1)
            {
                string[] parts = remaining.Split("by ");
                string title = parts[0];
                string artist = parts[1];

                IEnumerable<MusicService.Music> artistMatches = MusicService.Instance.Library.Where(music => Common.Similarity(music.Artist.ToLower(), artist) > 0.2f);

                if (artistMatches.Any())
                {
                    IEnumerable<MusicService.Music> titleMatches = artistMatches.Where(music => Common.Similarity(music.Title.ToLower(), title) > 0.5f);

                    if (titleMatches.Any())
                    {
                        MusicService.Music match = titleMatches.ElementAt(Common.MostSimilar(title, titleMatches.Select(music => music.Title).ToArray()));

                        RespondSuccess(match.Title, match.Artist);
                        MusicService.Instance.Play(match);
                        return;
                    }
                }
            }
            else
            {
                IEnumerable<MusicService.Music> matches = MusicService.Instance.Library.Where(music => Common.Similarity(music.Title.ToLower(), remaining) > 0.5f);

                if (matches.Any())
                {
                    MusicService.Music match = matches.ElementAt(Common.MostSimilar(remaining, matches.Select(music => music.Title).ToArray()));
                    RespondSuccess(match.Title);
                    MusicService.Instance.Play(match);
                    return;
                }
            }

            ChangeExpression(Expression.Frown);
            Respond(
            [
                "Sorry. I couldn't find that song.",
                "You don't have that song."
            ]);
        }

        private void RespondSuccess(string title, string? artist = null)
        {
            string response = $"{title}{(artist is not null ? $" by {artist}" : "")}";
            Respond(
            [
                $"Alright. Playing {response}.",
                $"Sure. Playing {response}.",
                $"Playing {response}."
            ]);
        }
    }
}
using GooeyWpf.Services;
using GooeyWpf.Synthesizer;
using GooeyWpf.Transcriber;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    [Command]
    internal class AppsCommand : InteractiveCommand
    {
        public AppsCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return text.StartsWith("open ") || text.StartsWith("launch ");
        }

        public override void Parse(string text)
        {
            if (Common.RemovePunctuation(text).ToLower().Contains("open the pod bay doors"))
            {
                ChangeExpression(Expression.Hal);
                Respond("I'm sorry, Dave. I'm afraid I can't do that.");
                return;
            }

            string appName = Common.RemovePunctuation(text[text.IndexOf(' ')..].Trim()).ToLower();

            IEnumerable<AppsService.App> matches = AppsService.Instance.Apps.Where(app => Common.Similarity(app.Name.ToLower(), appName) > 0.5f);

            if (matches.Any())
            {
                AppsService.App match = matches.ElementAt(Common.MostSimilar(appName, matches.Select(a => a.Name.ToLower()).ToArray()));
                ChangeExpression(Expression.Normal);
                Respond(
                [
                    $"Alright. Launching {match.Name}.",
                    $"Sure. Launching {match.Name}.",
                    $"Launching {match.Name}.",
                    $"Opening {match.Name}."
                ]);
                AppsService.LaunchApp(match);
                return;
            }

            ChangeExpression(Expression.Frown);
            Respond(
            [
                "Sorry, I couldn't find an app with that name.",
                "Sorry, I couldn't find that app.",
                "I'm afraid you don't have that app."
            ]);
        }
    }
}
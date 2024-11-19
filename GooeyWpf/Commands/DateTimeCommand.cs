using GooeyWpf.Services.Synthesizer;
using GooeyWpf.Services.Transcriber;
using System.Globalization;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    [Command]
    internal class DateTimeCommand : InteractiveCommand
    {
        private enum ResponseType
        {
            Time,
            Day,
            Date
        }

        public DateTimeCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.Similarities(text,
            [
                "time check",
                "what time is it",
                "what is the time",
                "what is the date today",
                "what is the date",
                "what is today",
                "what day is it today",
                "what day is it"
            ]) > 0.6f;
        }

        public override void Parse(string text)
        {
            DateTime now = DateTime.Now;
            if (text.Contains("time"))
            {
                Respond($"It is now {now.ToString("hh:mm tt")}");
            }
            else if (text.Contains("date"))
            {
                string ordinalSuffix = now.Day switch
                {
                    1 => "st",
                    2 => "nd",
                    3 => "rt",
                    _ => "th",
                };
                Respond($"It is the {now.Day}{ordinalSuffix} of {DateTimeFormatInfo.InvariantInfo.MonthNames[now.Month - 1]}.");
            }
            else if (text.Contains("day"))
            {
                Respond($"Today is {now.DayOfWeek}.");
            }
        }
    }
}
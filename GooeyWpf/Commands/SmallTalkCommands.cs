using GooeyWpf.Services.Synthesizer;
using GooeyWpf.Services.Transcriber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    [Command]
    internal class GoodMorningCommand : InteractiveCommand
    {
        enum TimeOfDay
        {
            Morning,
            Afternoon,
            Evening,
            Night
        }

        private TimeOfDay mentionedTimeOfDay;

        public GoodMorningCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            if (text.Contains("good morning"))
            {
                mentionedTimeOfDay = TimeOfDay.Morning;
            }
            else if (text.Contains("good afternoon"))
            {
                mentionedTimeOfDay = TimeOfDay.Afternoon;
            }
            else if (text.Contains("good evening"))
            {
                mentionedTimeOfDay = TimeOfDay.Evening;
            }
            else if (text.Contains("good night"))
            {
                mentionedTimeOfDay = TimeOfDay.Night;
            }
            else
                return false;

            return true;
        }

        public override void Parse(string text)
        {
            TimeOfDay correctTimeOfDay;
            int hour = DateTime.Now.TimeOfDay.Hours;
            if (hour >= 0 || hour <= 11)
                correctTimeOfDay = TimeOfDay.Morning;
            else if (hour >= 12 || hour <= 17)
                correctTimeOfDay = TimeOfDay.Afternoon;
            else if (hour >= 18 || hour <= 23)
                correctTimeOfDay = TimeOfDay.Evening;
            else
                correctTimeOfDay = TimeOfDay.Morning;

            string mentionedTimeOfDayName = Enum.GetName(typeof(TimeOfDay), mentionedTimeOfDay)?.ToLower() ?? "";
            string correctTimeOfDayName = Enum.GetName(typeof(TimeOfDay), correctTimeOfDay)?.ToLower() ?? "";
            if (mentionedTimeOfDay != correctTimeOfDay)
            {
                Respond([
                    $"It's currently {correctTimeOfDayName}, but good {mentionedTimeOfDay} nonetheless.",
                    $"Thank you, but it's currently {correctTimeOfDayName}. Good {correctTimeOfDayName}"
                    ]);
            }
            else
            {

                Respond(
                    [
                    $"Good {correctTimeOfDayName}.",
                    $"Good {correctTimeOfDayName} to you too."
                    ]);
            }
        }
    }

    internal class MyClass
    {

    }
}

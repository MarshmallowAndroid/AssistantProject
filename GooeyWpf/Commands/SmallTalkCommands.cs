﻿using GooeyWpf.Services;
using GooeyWpf.Synthesizer;
using GooeyWpf.Transcriber;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    [SmallTalkCommand]
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
            TimeOfDay correctTimeOfDay = TimeOfDay.Morning;
            int hour = DateTime.Now.TimeOfDay.Hours;
            if (hour >= 0 && hour <= 11)
                correctTimeOfDay = TimeOfDay.Morning;
            if (hour >= 12 && hour <= 17)
                correctTimeOfDay = TimeOfDay.Afternoon;
            if (hour >= 18 && hour <= 23)
                correctTimeOfDay = TimeOfDay.Evening;

            string mentionedTimeOfDayName = Enum.GetName(typeof(TimeOfDay), mentionedTimeOfDay)?.ToLower() ?? "";
            string correctTimeOfDayName = Enum.GetName(typeof(TimeOfDay), correctTimeOfDay)?.ToLower() ?? "";
            if (mentionedTimeOfDay != correctTimeOfDay)
            {
                Respond([
                    $"It's currently {correctTimeOfDayName}, but good {mentionedTimeOfDayName} nonetheless.",
                    $"Thank you, but it's currently {correctTimeOfDayName}. Good {correctTimeOfDayName}."
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

    [SmallTalkCommand]
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

    [SmallTalkCommand]
    internal class EmotionsCommand : InteractiveCommand
    {
        public EmotionsCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.Similarity("can you feel emotions", text) > 0.9f;
        }

        public override void Parse(string text)
        {
            Respond(
                [
                "I'm sorry, I'm just software and cannot feel emotions. However, I do specialize in speech recognition. I can understand what you're saying",
                "No. At least I don't think I do."
                ]);
        }
    }

    [SmallTalkCommand]
    internal class IntroCommand : InteractiveCommand
    {
        public IntroCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return Common.SimilarOrMatch(text, [
                "who are you", "where did you come from", "who are you and where did you come from",
                "introduce yourself"
             ], 0.6f);
        }

        public override void Parse(string text)
        {
            Respond(
                ["I was made in iACADEMY. My creators are J2MK; Jilliana, Jacob, Marc, and Kyle. They are students taking up Bachelors in Computer Science, with Specialization in Software Engineering. " +
                "I am their project for their Introduction to Artificial Intelligence class under Sir Bennett Tanyag."],
                ["I was made in eye-academy. My creators are J2MK; Jilliana, Jacob, Marc, and Kyle. They are students taking up Bachelors in Computer Science, with Specialization in Software Engineering. " +
                "I am their project for their Introduction to Artificial Intelligence class under Sir Bennett Tanyag."]);
        }
    }

    [SmallTalkCommand]
    internal class StopMusicCommand : InteractiveCommand
    {
        public StopMusicCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return text.Contains("stop ");
        }

        public override void Parse(string text)
        {
            MusicService.Instance.Stop();
            avatarController.StopVideo();
            Respond(
            [
                "Alright. Stopping music.", "Sure."
            ]);
        }
    }

    [SmallTalkCommand]
    class HelloCommand : InteractiveCommand
    {
        public HelloCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return text.Contains("hello");
        }

        public override void Parse(string text)
        {
            Respond(
                [
                "Hello!",
                "Hello there.",
                ]);
        }
    }

    [SmallTalkCommand]
    class WoodChuck : InteractiveCommand
    {
        public WoodChuck(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return text.Contains("how much wood would a woodchuck chuck if a woodchuck could chuck wood") ||
                text.Contains("how much wood would a wood chuck chuck if a wood chuck could chuck wood");
        }

        public override void Parse(string text)
        {
            Respond("He would chuck, he would, as much as he could, and chuck as much wood as a woodchuck would if a woodchuck could chuck wood.");
        }
    }

    //internal class PiperRebootCommand : InteractiveCommand
    //{
    //    public PiperRebootCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
    //    {
    //    }

    //    public override bool CommandMatch(string text)
    //    {
    //        text.Contains("i can't hear you anymore");
    //    }

    //    public override void Parse(string text)
    //    {
    //    }
    //}
}

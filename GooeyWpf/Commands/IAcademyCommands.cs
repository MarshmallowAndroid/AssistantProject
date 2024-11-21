using GooeyWpf.Synthesizer;
using GooeyWpf.Transcriber;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    [Command]
    class IAcademyDirectory : InteractiveCommand
    {
        public IAcademyDirectory(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return (text.Contains("can i ask") || text.Contains("ask about") || text.Contains("ask something about")) &&
                (text.Contains("iacademy"));
        }

        public override void Parse(string text)
        {
            Respond([
                "Sure!",
                "Sure, I'll try to answer as best as I can."]);
            Enter(QuestionsHandler);
        }

        void QuestionsHandler(object? sender, ITranscriber.TranscribeEventArgs e)
        {
            base.OnCommandTranscribe(sender, e);

            string text = Common.RemovePunctuation(e.Text).ToLower();

            Debug.WriteLine(text);

            if ((text.Contains("who is") || text.Contains("whos")) ||
                (text.Contains("who") && text.Contains("is")))
            {
                string person = "";
                string occupation = "";
                string? personPhonetic = null;
                string? occupationPhonetic = null;

                if (text.Contains("president of iacademy") ||
                    text.Contains("ceo of iacademy") ||
                    text.Contains("miss vanessa"))
                {
                    person = "Ms. Vanessa Tanco";
                    personPhonetic = "Miss Vanessa Tanco";
                    occupation = "current President and CEO, or chief executive officer, of iACADEMY";
                    avatarController.DisplayImage("iACADEMY/VanessaTanco.png");
                }
                else if (text.Contains("coo of iacademy")
                    || text.Contains("miss raquel"))
                {
                    person = "Ms. Raquel Wong";
                    personPhonetic = "Miss Raquel Wong";
                    occupation = "COO of iACADEMY.";
                    avatarController.DisplayImage("iACADEMY/RaquelPerezWong.png");
                }
                else if (text.Contains("dean of soc") ||
                    text.Contains("dean of the school of computing") ||
                    text.Contains("sir francisco"))
                {
                    person = "Sir Francisco Napalit";
                    personPhonetic = "Sir Francisco Nahpahlit";
                    occupation = "Dean of SOC or School of Computing.";
                    avatarController.DisplayImage("iACADEMY/FranciscoNapalit.png");
                }
                else if (text.Contains("dean of sbla") ||
                    text.Contains("dean of the school of business and liberal arts") ||
                    (text.Contains("sir john") || text.Contains("padua")))
                {
                    person = "Sir John Padua";
                    occupation = "Dean of SBLA or School of Business and Liberal Arts.";
                    avatarController.DisplayImage("iACADEMY/JohnPadua.png");
                }
                else if (text.Contains("dean of soda") ||
                    text.Contains("dean of the school of design and the arts") ||
                    text.Contains("sir jon"))
                {
                    person = "Sir Jon Cuyson";
                    occupation = "Dean of SODA or School of Design and the Arts.";
                    avatarController.DisplayImage("iACADEMY/JonCuyson.png");
                }
                else if (text.Contains("chairperson of it") ||
                    text.Contains("sir bennett"))
                {
                    person = "Sir Bennett Tanyag";
                    occupation = "Chairperson of the Information Technology Department";
                    avatarController.DisplayImage("iACADEMY/BennettTanyag.png");
                }
                else if (text.Contains("chairperson of game dev") || text.Contains("chairperson of game development") ||
                    text.Contains("sir carl"))
                {
                    person = "Sir Carl Louie So";
                    occupation = "Chairperson of the Game Development Department";
                    // no image :(
                }
                else if (text.Contains("chairperson of cs") || text.Contains("chairperson of computer science") ||
                    text.Contains("miss crisola"))
                {
                    person = "Ms. Crisola Tan";
                    personPhonetic = "Miss Crisola Tan";
                    occupation = "Chairperson of the Computer Science Department";
                    avatarController.DisplayImage("iACADEMY/CrisolaTan.png");
                }

                if (!string.IsNullOrEmpty(person) && !string.IsNullOrEmpty(occupation))
                {
                    Respond(
                        [
                        $"{person} is the {occupation}.",
                            $"The {occupation} is {person}."
                        ],
                        [
                        $"{personPhonetic ?? person} is the {occupationPhonetic ?? occupation}.",
                            $"The {occupationPhonetic ?? occupation} is {personPhonetic ?? person}."
                        ]);
                }
            }
            else if (text.Contains("when"))
            {
                if (text.Contains("iacademy") && text.Contains("established"))
                {
                    Respond(["iACADEMY was established "]);
                }
            }
            else if (text.Contains("where") || text.Contains("what floor") || text.Contains("which floor"))
            {
                if (text.Contains("gym") || text.Contains("gymnasium"))
                {
                    Respond("The gymnasium can be found on the lower penthouse, or LP.");
                }
                else if (text.Contains("clinic") || text.Contains("medical"))
                {
                    Respond("You can go to the clinic located on the ground floor, to the right after entering the turnstiles.");
                }
                else if (text.Contains("library") || text.Contains("read book") || text.Contains("research"))
                {
                    Respond("You can visit the library at the upper penthouse, or UP.");
                }
                else if (text.Contains("iacademy"))
                {
                    Respond("iACADEMY");
                }
            }
            else if (text.Contains("that will be all"))
            {
                Respond("I hope my answers, though limited, were sufficient.");
                Exit();
            }
        }

        //bool MatchAndGrab(string text, string[] matches, out string remaining)
        //{
        //    int matchIndex = 0;
        //    for (int i = 0; i < matches.Length; i++)
        //    {
        //        if ((matchIndex = text.IndexOf(matches[i], StringComparison.OrdinalIgnoreCase)) >= 0)
        //        {
        //            string match = matches[i];
        //            remaining = text[matchIndex..].Replace(match, "");
        //            return true;
        //        }
        //    }

        //    remaining = "";
        //    return false;
        //}
    }
}
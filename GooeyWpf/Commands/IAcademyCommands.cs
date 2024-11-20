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
            Respond("Sure! Ask away.");
            Enter(QuestionsHandler);
        }

        void QuestionsHandler(object? sender, ITranscriber.TranscribeEventArgs e)
        {
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
                    text.Contains("ceo of iacademy"))
                {
                    person = "Ms. Vanessa Tanco";
                    personPhonetic = "Miss Vanessa Tanco";
                    occupation = "current President and CEO, or chief executive officer, of iAcademy";
                }
                else if (text.Contains("coo of iacademy"))
                {
                    person = "Ms. Raquel Wong";
                    personPhonetic = "Miss Raquel Wong";
                    occupation = "COO of iAcademy.";
                }
                else if (text.Contains("dean of soc") ||
                    text.Contains("dean of the school of computing"))
                {
                    person = "Sir Francisco Napalit";
                    personPhonetic = "Sir Francisco Nahpahlit";
                    occupation = "Dean of SOC or School of Computing.";
                    avatarController.DisplayImage("iAcademy/FranciscoNapalit.png");
                }
                else if (text.Contains("dean of sbla") ||
                    text.Contains("dean of the school of business and liberal arts"))
                {
                    person = "Sir John Padua";
                    occupation = "Dean of SBLA or School of Business and Liberal Arts.";
                }
                else if (text.Contains("dean of soda") ||
                    text.Contains("dean of the school of design and the arts"))
                {
                    person = "Sir Jon Cuyson";
                    occupation = "Dean of SODA or School of Design and the Arts.";
                }
                else if (text.Contains("chairperson of it"))
                {
                    person = "Sir Bennett Tanyag";
                    occupation = "Chairperson of the Information Technology Department";
                    avatarController.DisplayImage("iAcademy/BennettTanyag.png");
                }
                else if (text.Contains("chairperson of game dev"))
                {
                    person = "Sir Carl Louie So";
                    occupation = "Chairperson of the Game Development Department";
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
            else if (text.Contains("thats all"))
            {
                Respond("I hope my answers, though limited, were sufficient.");
                Exit();
            }
        }
    }
}
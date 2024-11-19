using Synthesizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Transcriber;

namespace Gooey.Commands
{
    internal class KillCommand : Command
    {
        private readonly ISynthesizer synthesizer;
        private readonly Control control;
        private readonly CommandManager commandManager;

        public KillCommand(ISynthesizer synthesizer, Control control, CommandManager commandManager, ITranscriber transcriber) : base(transcriber)
        {
            this.synthesizer = synthesizer;
            this.control = control;
            this.commandManager = commandManager;
        }

        public override bool CommandMatch(string text)
        {
            return Common.Similarity(text, "kill yourself") > 0.5f;
        }

        public override void Parse(string text)
        {
            //string response = "";
            //control.Invoke(() =>
            //{
            //    control.Text = response;
            //});
            //synthesizer.Synthesize(response);
            transcriber.StopTranscribing();
            commandManager.Stop();
            Thread.Sleep(2000);
            Process.Start("shutdown.exe", "/s /t 0");
            Application.Exit();
        }

        protected override void OnCommandTranscribe(ITranscriber sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
        }
    }
}

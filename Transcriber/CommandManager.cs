using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcriber
{
    public class CommandManager
    {
        private readonly List<Command> commands = new();
        private readonly ITranscriber transcriber;
        private readonly string wakeCommand;
        private bool wakeWordDetected = false;
        private bool wakeResponded = false;

        public CommandManager(ITranscriber transcriber, string wakeCommand)
        {
            this.transcriber = transcriber;
            this.wakeCommand = wakeCommand;
            transcriber.Transcribe += Transcriber_Transcribe;
        }

        public event EventHandler<ITranscriber.TranscribeEventArgs>? Transcribe;
        public event EventHandler? Wake;

        public void RegisterCommand(Command command)
        {
            command.OriginalTranscribeEvent = Transcriber_Transcribe;
            commands.Add(command);
        }

        public void RegisterCommands(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                RegisterCommand(command);
            }
        }

        public void UnregisterCommands()
        {
            foreach (var command in commands)
            {
                command.OriginalTranscribeEvent -= Transcriber_Transcribe;
                commands.Remove(command);
            }
        }

        public void ManualInput(string command)
        {
            wakeWordDetected = true;
            wakeResponded = true;
            Transcriber_Transcribe(null, new ITranscriber.TranscribeEventArgs(command, 1.0f));
        }

        public void Stop()
        {
            transcriber.Transcribe -= Transcriber_Transcribe;
        }

        private int FirstIndexFrom(string findFrom, string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                int index = findFrom.IndexOf(strings[i]);
                if (index >= 0) return index;
            }
            return -1;
        }

        private void Transcriber_Transcribe(object? sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
            string text = eventArgs.Text.ToLower();

            if (text.Replace(",", "").StartsWith(wakeCommand, StringComparison.CurrentCultureIgnoreCase) ||
                Common.Similarity(wakeCommand, text) > 0.5f)
                wakeWordDetected = true;

            Transcribe?.Invoke(sender, eventArgs);
            if (wakeWordDetected)
            {
                if (!wakeResponded)
                {
                    Wake?.Invoke(this, EventArgs.Empty);
                    wakeResponded = true;
                }

                foreach (var command in commands)
                {
                    if (command.CommandMatch(text))
                    {
                        command.Parse(text);
                        wakeWordDetected = false;
                        wakeResponded = false;
                        break;
                    }
                }
            }
        }
    }
}

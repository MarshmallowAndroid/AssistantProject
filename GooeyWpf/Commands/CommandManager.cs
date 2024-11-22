using GooeyWpf.Transcriber;

namespace GooeyWpf.Commands
{
    public class CommandManager
    {
        private readonly List<Command> commands = new();
        private readonly List<Command> smalltalkCommands = new();
        private readonly ITranscriber transcriber;
        private readonly string wakeCommand;
        private readonly string[] variations;
        private bool listening = false;
        private bool wakeResponded = false;

        public CommandManager(ITranscriber transcriber, string wakeCommand, string[] variations)
        {
            this.transcriber = transcriber;
            this.wakeCommand = wakeCommand;
            this.variations = variations;
            transcriber.Transcribe += Transcriber_Transcribe;
        }

        public event EventHandler<ITranscriber.TranscribeEventArgs>? Transcribe;

        public event EventHandler? Wake;

        public event EventHandler? Sleep;

        public void RegisterCommand(Command command)
        {
            command.OriginalTranscribeEvent = Transcriber_Transcribe;
            commands.Add(command);
        }

        public void RegisterSmalltalkCommand(Command smalltalkCommand)
        {
            smalltalkCommand.OriginalTranscribeEvent = Transcriber_Transcribe;
            smalltalkCommands.Add(smalltalkCommand);
        }

        public void RegisterCommands(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                RegisterCommand(command);
            }
        }

        public void RegisterSmalltalkCommands(IEnumerable<Command> smalltalkCommands)
        {
            foreach (var command in smalltalkCommands)
            {
                RegisterSmalltalkCommand(command);
            }
        }

        public void UnregisterCommands()
        {
            foreach (var command in commands)
            {
                command.OriginalTranscribeEvent -= Transcriber_Transcribe;
                commands.Remove(command);
            }

            foreach (var smalltalkCommand in smalltalkCommands)
            {
                smalltalkCommand.OriginalTranscribeEvent -= Transcriber_Transcribe;
                smalltalkCommands.Remove(smalltalkCommand);
            }
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

            text.Replace("eye academy", "iacademy")
                .Replace("hi academy", "iacademy")
                .Replace("i academy", "iacademy")
                .Replace("high academy", "iacademy")
                .Replace("the academy", "iacademy")
                .Replace("that academy", "iacademy")
                .Replace("ai academy", "iacademy")
                .Replace("my academy", "iacademy");

            //foreach (var variation in variations)
            //{
            //    if (Common.RemovePunctuation(text).Contains(variation, StringComparison.CurrentCultureIgnoreCase))
            //    {
            //        listening = true;
            //        break;
            //    }
            //}

            string remaining = "";
            if (!listening)
            {
                foreach (var variation in variations)
                {
                    string textDumb = Common.RemovePunctuation(text);
                    if (textDumb.Contains(variation, StringComparison.CurrentCultureIgnoreCase))
                    {
                        string lowerVariation = variation.ToLower();
                        remaining = textDumb[textDumb.IndexOf(lowerVariation)..].Replace(variation.ToLower(), "").Trim();
                        listening = true;
                        break;
                    }
                }
                remaining = remaining.Trim();
            }

            Transcribe?.Invoke(sender, eventArgs);
            if (listening)
            {
                if (string.IsNullOrEmpty(remaining))
                {
                    if (!wakeResponded)
                    {
                        Wake?.Invoke(this, EventArgs.Empty);
                        wakeResponded = true;
                    }
                    remaining = text;
                }

                if (!ProcessCommand(remaining, commands))
                    ProcessCommand(remaining, smalltalkCommands);
            }
            else
            {
                ProcessCommand(text, smalltalkCommands);
            }
        }

        private bool ProcessCommand(string commandString, IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                if (command.CommandMatch(commandString))
                {
                    command.Parse(commandString);
                    listening = false;
                    wakeResponded = false;
                    Sleep?.Invoke(this, EventArgs.Empty);
                    return true;
                }
            }
            return false;
        }
    }
}
using GooeyWpf.Services.Synthesizer;
using GooeyWpf.Services.Transcriber;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    internal class DirectCommand : InteractiveCommand
    {
        public DirectCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : base(transcriber, synthesizer, chatLog, avatarController)
        {
        }

        public override bool CommandMatch(string text)
        {
            return false;
        }

        public override void Parse(string text)
        {
        }

        public void DirectResponse(string response) => Respond(response);

        public void DirectResponse(string displayResponse, string spokenResponse) => Respond(displayResponse, spokenResponse);

        public void DirectResponse(string[] responses) => Respond(responses);

        public void DirectResponse(string[] displayResponses, string[] spokenResponses) => Respond(displayResponses, spokenResponses);
    }
}
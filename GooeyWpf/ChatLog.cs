using System.Globalization;

namespace GooeyWpf
{
    internal class ChatLog(ChatLog.ChatSpeaker speaker, string log, string language = "en")
    {
        public enum ChatSpeaker
        {
            Program,
            User
        }

        public ChatSpeaker Speaker { get; } = speaker;

        public string SpeakerName => Speaker switch
        {
            ChatSpeaker.Program => Assistant.AssistantName + ": ",
            ChatSpeaker.User => $"You {(Language != "en" ? $"({CultureInfo.GetCultureInfo(Language).DisplayName})" : "")}: ",
            _ => "Unknown: "
        };

        public string Log { get; } = log;

        public string Language { get; } = language;
    }
}
using GooeyWpf.Services;
using GooeyWpf.Services.Synthesizer;
using GooeyWpf.Services.Transcriber;
using System.Windows;
using System.Windows.Controls;

namespace GooeyWpf.Commands
{
    internal abstract class InteractiveCommand(ITranscriber transcriber, ISynthesizer synthesizer, ListBox chatLog, AvatarController avatarController) : Command(transcriber)
    {
        protected readonly ISynthesizer synthesizer = synthesizer;
        protected readonly ListBox chatLog = chatLog;
        protected readonly AvatarController avatarController = avatarController;

        protected override void OnCommandTranscribe(object? sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
            chatLog.Dispatcher.Invoke(() =>
            {
                chatLog.Items.Add(new ChatLog(ChatLog.ChatSpeaker.User, eventArgs.Text));
            });
        }

        protected void Respond(string text)
        {
            chatLog.Dispatcher.Invoke(() =>
            {
                chatLog.Items.Add(new ChatLog(ChatLog.ChatSpeaker.Program, text));
                MainWindow.ScrollToEnd(chatLog);
            });
            synthesizer.Synthesize(text);
        }

        protected void Respond(string displayText, string spokenText)
        {
            chatLog.Dispatcher.Invoke(() =>
            {
                chatLog.Items.Add(new ChatLog(ChatLog.ChatSpeaker.Program, displayText));
                MainWindow.ScrollToEnd(chatLog);
            });
            synthesizer.Synthesize(spokenText);
        }

        protected void Respond(string[] texts)
        {
            int textIndex = RandomService.Instance.Next(0, texts.Length);
            Respond(texts[textIndex]);
        }

        protected void Respond(string[] displayTexts, string[] spokenTexts)
        {
            int displayTextIndex = RandomService.Instance.Next(0, displayTexts.Length);
            int spokenTextIndex = RandomService.Instance.Next(0, spokenTexts.Length);
            Respond(displayTexts[displayTextIndex], spokenTexts[spokenTextIndex]);
        }

        protected void ChangeExpression(Expression expression)
        {
            chatLog.Dispatcher.Invoke(() =>
            {
                avatarController.ChangeExpression(expression);
            });
        }
    }
}
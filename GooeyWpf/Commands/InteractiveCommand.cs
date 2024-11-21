using GooeyWpf.Services;
using GooeyWpf.Synthesizer;
using GooeyWpf.Transcriber;
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
            //chatLog.Dispatcher.Invoke(() =>
            //{
            //    chatLog.Items.Add(new ChatLog(ChatLog.ChatSpeaker.User, eventArgs.Text));
            //});
        }

        protected void Respond(string text, int delayMs = 0, Action<Task>? postAction = null)
        {
            chatLog.Dispatcher.Invoke(() =>
            {
                chatLog.Items.Add(new ChatLog(ChatLog.ChatSpeaker.Program, text));
                MainWindow.ScrollToEnd(chatLog);
            });
            synthesizer.Synthesize(text);

            if (postAction is not null)
            {
                Task.Delay(delayMs).ContinueWith(postAction);
            }
        }

        protected void Respond(string displayText, string spokenText, int delayMs = 0, Action<Task>? postAction = null)
        {
            chatLog.Dispatcher.Invoke(() =>
            {
                chatLog.Items.Add(new ChatLog(ChatLog.ChatSpeaker.Program, displayText));
                MainWindow.ScrollToEnd(chatLog);
            });
            synthesizer.Synthesize(spokenText);

            if (postAction is not null)
            {
                Task.Delay(delayMs).ContinueWith(postAction);
            }
        }

        protected void Respond(string[] texts, int delayMs = 0, Action<Task>? postAction = null)
        {
            int textIndex = RandomService.Instance.Next(0, texts.Length);
            Respond(texts[textIndex], delayMs, postAction);
        }

        protected void Respond(string[] displayTexts, string[] spokenTexts, int delayMs = 0, Action<Task>? postAction = null)
        {
            int textIndex = RandomService.Instance.Next(0, displayTexts.Length);
            Respond(displayTexts[textIndex], spokenTexts[textIndex], delayMs, postAction);
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
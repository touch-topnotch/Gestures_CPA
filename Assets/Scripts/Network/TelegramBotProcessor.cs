using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UnityEngine;
using File = System.IO.File;

namespace Scripts.Network
{
    public static class TelegramBotProcessor
    {
        private const string botToken = "7086788178:AAEmDpBcXwSh9QEZZ5MyLBtNg62mVjZ6PMg";
        private const string chatId = "1042330275";
        private static TelegramBotClient bot = new TelegramBotClient(botToken);
        public static Action<string> onMessageReceived;
        private static CancellationTokenSource cts = new CancellationTokenSource();

        private static SynchronizationContext
            unityMainThreadContext; // Контекст синхронизации для основного потока Unity

        static TelegramBotProcessor()
        {
            unityMainThreadContext = SynchronizationContext.Current; // Инициализация контекста синхронизации
        }

       
        public static async Task SendToTelegramAsync(string filePath)
        {
            var tcs = new TaskCompletionSource<bool>();
            unityMainThreadContext.Post(async _ =>
            {
                try
                {
                    var stream = File.OpenRead(filePath);
                    InputFile file = new InputFileStream(stream, filePath.Split('/')[^1]);
                    await bot.SendDocumentAsync(chatId, file);
                    tcs.SetResult(true); // Сигнал об успешном выполнении
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex); // Сигнал об ошибке
                }
            }, null);
            await tcs.Task; // Ожидание завершения асинхронной операции
        }
        public static void StartReceiving()
        {
            cts.Cancel();
            cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            bot.StartReceiving<MyUpdateHandler>(new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message } },
                cancellationToken);
        }

        public static void StopReceiving()
        {
            cts.Cancel();
        }

        private class MyUpdateHandler : IUpdateHandler
        {
            public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
                CancellationToken cancellationToken)
            {
                unityMainThreadContext.Post(_ => Debug.Log("updating..."), null);
                if (update.Type == UpdateType.Message && update.Message.Text != null)
                {
                    onMessageReceived?.Invoke(update.Message.Text);
                    // delete message
                    if (update.Message.Text.Contains("Char") || update.Message.Text.Contains("Gest"))
                        await botClient.DeleteMessageAsync(chatId, update.Message.MessageId);
                }
            }

            public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
                CancellationToken cancellationToken)
            {
                unityMainThreadContext.Post(_ => Debug.LogError(exception), null);
                return Task.CompletedTask;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
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
        public static Action<Message> onMessageReceived;
        private const string botToken = "7086788178:AAEmDpBcXwSh9QEZZ5MyLBtNg62mVjZ6PMg";
        private const string chatId = "1042330275";
        
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static readonly TelegramBotClient bot = new TelegramBotClient(botToken);
        private static readonly SynchronizationContext
            unityMainThreadContext; // Контекст синхронизации для основного потока Unity
    
        private static DateTime startTime;
        public static List<Message> receivedMessages = new List<Message>();
        static TelegramBotProcessor()
        {
            unityMainThreadContext = SynchronizationContext.Current; // Инициализация контекста синхронизации
        }

        public static void DeleteMessage(int messageId)
        {
            bot.DeleteMessageAsync(TelegramBotProcessor.chatId, messageId);
        }

       
        public static async Task SendFileToTelegram(string fileName, string value)
        {
            Debug.Log("Trying to send...");
    
            try
            {
                await SendTextToTelegram("Fratello, presto invierò json con gesti");
                
                byte[] utf16Bytes = System.Text.Encoding.Unicode.GetBytes(value);

                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    await fs.WriteAsync(utf16Bytes, 0, utf16Bytes.Length);
                    fs.Position = 0; // Reset the file stream position to the beginning
            
                    await bot.SendDocumentAsync(chatId, new InputFileStream(fs, fileName: fileName));
                }
        
                File.Delete(fileName);
        
                Debug.Log("File sent successfully and deleted.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending file: {ex}");
                throw;
            }
        }

        public static async Task SendTextToTelegram(string text)
        {
            var tcs = new TaskCompletionSource<bool>();
            unityMainThreadContext.Post(async _ =>
            {
                try
                {
                    await bot.SendTextMessageAsync(chatId, text);
                    tcs.SetResult(true); // Сигнал об успешном выполнении
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex); // Сигнал об ошибке
                }
            }, null);// Ожидание завершения асинхронной операции
            await tcs.Task;
        }
        public static void StartReceiving()
        {
            Debug.Log("Starting to receive updates...");

            // Cancel any ongoing polling
            if (cts is { IsCancellationRequested: false })
            {
                Debug.Log("Cancelling previous polling...");
                cts.Cancel();
            }

            // Create a new CancellationTokenSource for the new polling
            cts = new CancellationTokenSource();

            // Start receiving updates
            bot.StartReceiving<MyUpdateHandler>(
                new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message } }, cts.Token);

            Debug.Log("Polling started.");
        }

        public static void StopReceiving()
        {
            Debug.Log("Stopping updates...");

            // Cancel the current polling
            if (cts is { IsCancellationRequested: false })
            {
                cts.Cancel();
            }

            Debug.Log("Updates stopped.");
        }


        
        private class MyUpdateHandler : IUpdateHandler
        {
            private void SendUnityTask(Message message)
            {
                receivedMessages.Add(message);
                onMessageReceived?.Invoke(message);
            }

          
            public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
                CancellationToken cancellationToken)
            {
                if (update.Type == UpdateType.Message && update.Message?.Text != null && update.Message.Date > startTime )
                {
                
                    unityMainThreadContext.Post(_ => SendUnityTask(message: update.Message), null);
                }
            }

            public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
                CancellationToken cancellationToken)
            {
                unityMainThreadContext.Post(_ => Debug.Log(exception), null);
                return Task.CompletedTask;
            }
        }
    }
}

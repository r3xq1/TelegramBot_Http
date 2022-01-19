namespace TelegramBotEx
{
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using System.Threading;

    public static class PanelBot
    {
        // Подключение к боту
        private const string BOT_TOKEN = @"Token"; // Ваш токен от бота
        private static int LastUpdateID = 0; // Ваш ID чат канала

        // Таймер срабатывания метода для проверки команд (в секундах)
        public static void TimeExecuteUpdate(TimeSpan ts)
        {
            try
            {
                using var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, MutEx.GetGUID(), out bool createdNew);
                if (!createdNew) { waitHandle.Set(); return; }
                bool signaled;
                do
                {
                    signaled = waitHandle.WaitOne(ts);
                    Inizialize();
                }
                while (!signaled);
            }
            catch (Exception) { }
        }
        // Для ввода текста пользователя в боте без регистра
        private static bool Contains(this string source, string value, StringComparison comp) => source.IndexOf(value, comp) > -1;

        // Метод для получение данных от бота и отправки действия
        private static void Inizialize()
        {
            using WebClient webClient = new();
            string response = webClient.DownloadString($"https://api.telegram.org/bot{BOT_TOKEN}/getUpdates?offset={LastUpdateID + 1}");

            foreach (JSONNode text in SimpleJSON.Parse(response)["result"]?.AsArray)
            {
                string message = text["message"]["text"].Value; // Полученная команда от бота
                int ToTheChannel = text["message"]["chat"]["id"].AsInt; // Чат ID канала
                LastUpdateID = text["update_id"].AsInt; // Последний id полученного диалога

                // Проверяем команду полученную из канала и выполняем определённые действия.
                if (message.Contains("/start", StringComparison.CurrentCultureIgnoreCase))
                {
                    SendMessage($"Привет, чувак ", ToTheChannel); // Отправляем команду
                }
                else if (message.Contains("/update", StringComparison.CurrentCultureIgnoreCase))
                {
                    SendMessage("Обновились", ToTheChannel);  // Отправляем команду
                }
                else
                {
                    SendMessage("Неизвестная команда", ToTheChannel);  // Отправляем команду
                }
                // и.т.д
                #region Без Contains
                //if (text["message"]["text"].Equals("/start"))
                //{
                //    SendMessage("Привет, я бот", text["message"]["chat"]["id"].AsInt);
                //}
                //else if (text["message"]["text"].Equals("/update"))
                //{
                //    SendMessage("Обновили страницу!", text["message"]["chat"]["id"].AsInt);
                //}
                //else
                //{
                //    SendMessage("Неизвестная команда", text["message"]["chat"]["id"].AsInt);
                //}
                #endregion
            }
        }

        // Отправка сообщения боту
        private static void SendMessage(string message, int chatId)
        {
            using WebClient webClient = new();
            NameValueCollection pars = new()
            {
                { "text", message }, // Текст сообщения для отправки
                { "chat_id", chatId.ToString() } // ID чат канала
            };
            byte[] result = webClient.UploadValues($"https://api.telegram.org/bot{BOT_TOKEN}/sendMessage", pars);
            // тут можно вывести результат отправки если надо
        }
    }
}
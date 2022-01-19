namespace TelegramBotEx
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public static class UploadEx
    {
        /* Справочник: https://tlgrm.ru/docs/bots/api */
        /* Максимальный размер отправки 10 МБ для фотографий, 50 МБ для других файлов */

        #region Использование отправки при помощи Http 

        /// <summary>
        /// Метод для отправки файла на сервер телеграм канала
        /// </summary>
        /// <param name="token">Токен бота</param>
        /// <param name="chatID">ID чат канала</param>
        /// <param name="filePath">Полный путь до файла</param>
        /// <param name="message">Заголовок сообщения</param>
        /// <returns>Результат отправки</returns>
        public static async Task Send_Http_File(string token, string chatID, string filePath, string message)
        {
            // Если файла нет, выход из функции
            if (!File.Exists(filePath)) { return; }

            using MultipartFormDataContent form = new()
            {
               { new StringContent(message, Encoding.UTF8), "caption" }, // Заголовок сообщения
               { new StringContent(chatID, Encoding.UTF8), "chat_id" }, // ID - чат канала
               { new ByteArrayContent(File.ReadAllBytes(filePath)), "document", Path.GetFileName(filePath)} // Файл для отправки
            };
            using HttpClient client = new();
            string address = $"https://api.telegram.org/bot{token}/sendDocument"; // API данные для отправки файла через sendDocument
            using HttpResponseMessage result = await client.PostAsync(address, form).ConfigureAwait(false); // Отправка файла на сервер
            Console.WriteLine(result.StatusCode == HttpStatusCode.OK ? "File Send Successfully" : "Not Send"); // Получение результата
        }

        /// <summary>
        /// <br>Метод для отправки файл(а)ов в канал с ответом</br>
        /// </summary>
        /// <param name="token">Токен бота</param>
        /// <param name="chatID">ID чат канала</param>
        /// <param name="filePath">Полный путь до файла</param>
        /// <param name="message">Заголовок сообщения</param>
        /// <returns>Ответ от сервера</returns>
        public static async Task<string> Send_Http_FileStr(string token, string chatID, string filePath, string message)
        {
            // Содержимое данных для отправки на сервер
            using MultipartFormDataContent form = new()
            {
               { new StringContent(message, Encoding.UTF8), "caption" }, // Заголовок сообщения
               { new StringContent(chatID, Encoding.UTF8), "chat_id" }, // ID - чат канала
               { new ByteArrayContent(File.ReadAllBytes(filePath)), "document", Path.GetFileName(filePath)} // Файл для отправки
            };
            using HttpClient client = new();
            string address = $"https://api.telegram.org/bot{token}/sendDocument"; // API данные для отправки файла через sendDocument
            using HttpResponseMessage result = await client.PostAsync(address, form).ConfigureAwait(false); // Отправка файла на сервер
            Console.WriteLine(result.StatusCode == HttpStatusCode.OK ? "File Send Successfully" : "Not Send"); // Получение результата по отправки                                                                                                           
            return await result.Content.ReadAsStringAsync(); // Ответ от сервера
        }

        /// <summary>
        /// Метод для отправки сообщения на сервер телеграм канала через <b>HttpClient</b>
        /// </summary>
        /// <param name="token">Токен бота</param>
        /// <param name="chatID">ID чат канала</param>
        /// <param name="message">Текст сообщения для отправки</param>
        /// <param name="htmlFormat">Текст в формате Html</param>
        /// <returns>Результат отправки</returns>
        public static async Task SendMessage(string token, string chatID, string message, bool htmlFormat)
        {
            using HttpClient httpClient = new();
            string link = $"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatID}&parse_mode=MarkDown&text={(htmlFormat ? $"`{message}`" : message)}";

            using HttpResponseMessage result = await httpClient.GetAsync(link, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            Console.WriteLine(result.StatusCode == HttpStatusCode.OK ? "Send" : "Fail");
        }

        /// <summary>
        /// Метод для отправки сообщения на сервер телеграм канала через WebRequest
        /// </summary>
        /// <param name="token">Токен бота</param>
        /// <param name="chatID">ID чат канала</param>
        /// <param name="message">Текст сообщения для отправки</param>
        /// <param name="htmlFormat">Текст в формате Html</param>
        /// <returns></returns>
        public static bool SendHttpWeb(string token, string chatID, string message, bool htmlFormat)
        {
            string link = $"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatID}&parse_mode=MarkDown&text={(htmlFormat ? $"`{message}`" : message)}";
            Uri uri = new(link, UriKind.Absolute);
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            if (request.RequestUri.IsAbsoluteUri)
            {
                try
                {
                    using HttpWebResponse response = request?.GetResponse() as HttpWebResponse;
                    return response.StatusCode == HttpStatusCode.OK;
                }
                catch { throw new Exception("Ошибка отправки!"); }
            }
            return false;
        }

        #endregion

        #region Использование отправки при помощи WebClient

        public static void WebSendFile(string token, string chatID, string filePath, string message)
        {
            string boundary = $"------------------------{DateTime.Now.Ticks:x}";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using WebClient webClient = new();
                webClient.Headers.Add("Content-Type", $"multipart/form-data; boundary={boundary}");

                StringBuilder contentBuilder = new();
                contentBuilder.AppendLine($"--{boundary}");
                contentBuilder.AppendLine($"Content-Disposition: form-data; name=\"document\"; filename=\"{Path.GetFileName(filePath)}\"");
                contentBuilder.AppendLine($"Content-Type: application/octet-stream\r\n");
                contentBuilder.AppendLine($"{webClient.Encoding.GetString(File.ReadAllBytes(filePath))}");
                contentBuilder.AppendLine($"\r\n--{boundary}--\r\n");

                Uri uri = new($"https://api.telegram.org/bot{token}/sendDocument?chat_id={chatID}&caption={message}");
                byte[] result = webClient.UploadData(uri, "POST", webClient.Encoding.GetBytes(contentBuilder?.ToString()));
            }
            catch { }
        }

        public static void SendMessage(string token, string chatID, string message)
        {
            using WebClient webClient = new();
            NameValueCollection pars = new()
            {
                { "text", message }, // текст который отправляем
                { "chat_id", chatID?.ToString() } // чат id на который идет ответ
            };
            webClient.UploadValues($"https://api.telegram.org/bot{token}/sendMessage", pars);
        }

        #endregion
    }
}
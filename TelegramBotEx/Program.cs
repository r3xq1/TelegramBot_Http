namespace TelegramBotEx
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class Program
    {
        // Путь до текущей директории (для теста отправки файла)
        private static readonly string CurrDir = Environment.CurrentDirectory;

        [STAThread]
        public static void Main(string[] args)
        {
            Console.Title = "Telegram Bot HttpClient"; // Заголовок консоли
            PanelBot.TimeExecuteUpdate(TimeSpan.FromSeconds(1)); // Получение комманд от бота
           // MainAsync().GetAwaiter().GetResult(); // Выполнение асинхронного метода 
            //UploadEx.WebSendFile("Token", "ChatID", Path.Combine(CurrDir, "File.txt"), "MessageTest"); 
            //UploadEx.SendHttpWeb("Token", "ChatID", "Привет, r3xq1", true);

            Console.Read();
        }

        [STAThread]
        public static async Task MainAsync()
        {
            await UploadEx.SendMessage("Token", "ChatID", "Привет, r3xq1", true); // первый метод отправки
            //await UploadEx.Send_Http_File("Token", "ChatID", Path.Combine(CurrDir, "File.txt"), "Текст сообщения"); // второй метод отправки
            //string test = await UploadEx.Send_Http_FileStr("Token", "ChatID", Path.Combine(CurrDir, "Путь до файла"), "Любой текст сообщения"); // третий метод отправки
        }
    }
}
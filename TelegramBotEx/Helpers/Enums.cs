namespace TelegramBotEx
{
    using System;

    public static class Enums
    {
        #region Для Json
        [Flags]
        public enum JSONNodeType : int
        {
            Array = 1,
            Object = 2,
            String = 3,
            Number = 4,
            NullValue = 5,
            Boolean = 6,
            None = 7,
            Custom = 0xFF,
        }
        [Flags]
        public enum JSONTextMode
        {
            Compact,
            Indent
        }
        #endregion
    }
}

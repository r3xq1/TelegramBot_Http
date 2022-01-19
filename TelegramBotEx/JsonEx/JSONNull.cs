namespace TelegramBotEx
{
    using System.Text;

    public partial class JSONNull : JSONNode
    {
        private static readonly JSONNull m_StaticInstance = new();
        public static bool reuseSameInstance = true;
        public static JSONNull CreateOrGet => reuseSameInstance ? m_StaticInstance : new JSONNull();
        private JSONNull() { }

        public override Enums.JSONNodeType Tag => Enums.JSONNodeType.NullValue;
        public override bool IsNull => true;
        public override Enumerator GetEnumerator() => new();

        public override string Value
        {
            get => "null";
            set { }
        }
        public override bool AsBool
        {
            get => false;
            set { }
        }
        public override JSONNode Clone() => CreateOrGet;

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is JSONNull;
        public override int GetHashCode() => 0;

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, Enums.JSONTextMode aMode)
        {
            aSB.Append("null");
        }
    }
}
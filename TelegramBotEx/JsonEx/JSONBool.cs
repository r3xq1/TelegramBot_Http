namespace TelegramBotEx
{
    using System.Text;

    public partial class JSONBool : JSONNode
    {
        private bool m_Data;
        public override Enums.JSONNodeType Tag => Enums.JSONNodeType.Boolean;
        public override bool IsBoolean => true;
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public override string Value
        {
            get => m_Data.ToString();
            set
            {
                if (bool.TryParse(value, out bool v))
                {
                    m_Data = v;
                }
            }
        }
        public override bool AsBool
        {
            get => m_Data;
            set => m_Data = value;
        }

        public JSONBool(bool aData) => m_Data = aData;
        public JSONBool(string aData) => Value = aData;

        public override JSONNode Clone() => new JSONBool(m_Data);

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, Enums.JSONTextMode aMode)
        {
            aSB.Append(m_Data ? "true" : "false");
        }
        public override bool Equals(object obj) => obj != null && obj is bool boolean && m_Data == boolean;

        public override int GetHashCode() => m_Data.GetHashCode();
        public override void Clear()
        {
            m_Data = false;
        }
    }
}
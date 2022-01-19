namespace TelegramBotEx
{
    using System.Text;

    public partial class JSONString : JSONNode
    {
        private string m_Data;
        public override Enums.JSONNodeType Tag => Enums.JSONNodeType.String;
        public override bool IsString => true;
        public override Enumerator GetEnumerator() => new();

        public override string Value
        {
            get => m_Data;
            set => m_Data = value;
        }

        public JSONString(string aData)
        {
            m_Data = aData;
        }
        public override JSONNode Clone() => new JSONString(m_Data);

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, Enums.JSONTextMode aMode)
        {
            aSB.Append('\"').Append(Escape(m_Data)).Append('\"');
        }
        public override bool Equals(object obj) => base.Equals(obj) || (obj is string s ? m_Data == s : (JSONString)obj != null && m_Data == ((JSONString)obj).m_Data);
        public override int GetHashCode() => m_Data.GetHashCode();
        public override void Clear()
        {
            m_Data = "";
        }
    }
}
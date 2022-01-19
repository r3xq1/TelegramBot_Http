namespace TelegramBotEx
{
    using System;
    using System.Globalization;
    using System.Text;

    public partial class JSONNumber : JSONNode
    {
        private double m_Data;
        public override Enums.JSONNodeType Tag => Enums.JSONNodeType.Number;
        public override bool IsNumber => true;
        public override Enumerator GetEnumerator() => new();

        public override string Value
        {
            get => m_Data.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
                {
                    m_Data = v;
                }
            }
        }
        public override double AsDouble
        {
            get => m_Data;
            set => m_Data = value;
        }
        public override long AsLong
        {
            get => (long)m_Data;
            set => m_Data = value;
        }
        public override ulong AsULong
        {
            get => (ulong)m_Data;
            set => m_Data = value;
        }

        public JSONNumber(double aData)
        {
            m_Data = aData;
        }
        public JSONNumber(string aData)
        {
            Value = aData;
        }
        public override JSONNode Clone() => new JSONNumber(m_Data);
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, Enums.JSONTextMode aMode)
        {
            aSB.Append(Value);
        }
        private static bool IsNumeric(object value) => value is int or uint or float or double or decimal or long or ulong or short or ushort or sbyte or byte;
        public override bool Equals(object obj) => obj != null && (base.Equals(obj) || ((obj as JSONNumber) != null ? m_Data == (obj as JSONNumber).m_Data : IsNumeric(obj) && Convert.ToDouble(obj) == m_Data));
        public override int GetHashCode() => m_Data.GetHashCode();
        public override void Clear()
        {
            m_Data = 0;
        }
    }
}
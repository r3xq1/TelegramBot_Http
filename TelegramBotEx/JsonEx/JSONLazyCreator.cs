namespace TelegramBotEx
{
    using System.Text;

    // End of JSONNull

    internal partial class JSONLazyCreator : JSONNode
    {
        private JSONNode m_Node = null;
        private readonly string m_Key = null;
        public override Enums.JSONNodeType Tag => Enums.JSONNodeType.None;
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public JSONLazyCreator(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
        }

        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
        }

        private T Set<T>(T aVal) where T : JSONNode
        {
            m_Node.Add(m_Key == null ? aVal : m_Key, aVal);
            m_Node = null; // Be GC friendly.
            return aVal;
        }

        public override JSONNode this[int aIndex]
        {
            get => new JSONLazyCreator(this);
            set => Set(new JSONArray()).Add(value);
        }

        public override JSONNode this[string aKey]
        {
            get => new JSONLazyCreator(this, aKey);
            set => Set(new JSONObject()).Add(aKey, value);
        }

        public override void Add(JSONNode aItem) => Set(new JSONArray()).Add(aItem);
        public override void Add(string aKey, JSONNode aItem) => Set(new JSONObject()).Add(aKey, aItem);
        public static bool operator ==(JSONLazyCreator a, object b) => b == null || ReferenceEquals(a, b);
        public static bool operator !=(JSONLazyCreator a, object b) => !(a == b);
        public override bool Equals(object obj) => obj == null || ReferenceEquals(this, obj);
        public override int GetHashCode() => 0;

        public override int AsInt
        {
            get { Set(new JSONNumber(0)); return 0; }
            set => Set(new JSONNumber(value));
        }
        public override float AsFloat
        {
            get { Set(new JSONNumber(0.0f)); return 0.0f; }
            set => Set(new JSONNumber(value));
        }
        public override double AsDouble
        {
            get { Set(new JSONNumber(0.0)); return 0.0; }
            set => Set(new JSONNumber(value));
        }
        public override long AsLong
        {
            get
            {
                if (longAsString)
                {
                    Set(new JSONString("0"));
                }
                else
                {
                    Set(new JSONNumber(0.0));
                }

                return 0L;
            }
            set
            {
                if (longAsString)
                {
                    Set(new JSONString(value.ToString()));
                }
                else
                {
                    Set(new JSONNumber(value));
                }
            }
        }

        public override ulong AsULong
        {
            get
            {
                if (longAsString)
                {
                    Set(new JSONString("0"));
                }
                else
                {
                    Set(new JSONNumber(0.0));
                }
                return 0;
            }
            set
            {
                if (longAsString)
                {
                    Set(new JSONString(value.ToString()));
                }
                else
                {
                    Set(new JSONNumber(value));
                }
            }
        }

        public override bool AsBool
        {
            get 
            { 
                Set(new JSONBool(false));
                return false;
            }
            set => Set(new JSONBool(value));
        }

        public override JSONArray AsArray => Set(new JSONArray());
        public override JSONObject AsObject => Set(new JSONObject());

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, Enums.JSONTextMode aMode)
        {
            StringBuilder result = aSB.Append("null");
        }
    }
}
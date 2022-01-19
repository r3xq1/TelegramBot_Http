namespace TelegramBotEx
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public abstract partial class JSONNode
    {
        #region Enumerators

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator
        {
            private enum Type { None, Array, Object }
            private readonly Type type;
            private Dictionary<string, JSONNode>.Enumerator m_Object;
            private List<JSONNode>.Enumerator m_Array;
            public bool IsValid => type != Type.None;
            public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
            {
                type = Type.Array;
                m_Object = default;
                m_Array = aArrayEnum;
            }
            public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
            {
                type = Type.Object;
                m_Object = aDictEnum;
                m_Array = default;
            }
            public KeyValuePair<string, JSONNode> Current => type switch
            {
                Type.Array => new KeyValuePair<string, JSONNode>(string.Empty, m_Array.Current),
                Type.Object => m_Object.Current,
                _ => new KeyValuePair<string, JSONNode>(string.Empty, null),
            };
            public bool MoveNext() => type switch
            {
                Type.Array => m_Array.MoveNext(),
                Type.Object => m_Object.MoveNext(), _ => false,
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ValueEnumerator
        {
            private Enumerator m_Enumerator;
            public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
            public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
            public ValueEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
            public JSONNode Current => m_Enumerator.Current.Value;
            public bool MoveNext() => m_Enumerator.MoveNext();
            public ValueEnumerator Enumerator => this;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyEnumerator
        {
            private Enumerator m_Enumerator;
            public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
            public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
            public KeyEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
            public string Current => m_Enumerator.Current.Key;
            public bool MoveNext => m_Enumerator.MoveNext(); public KeyEnumerator Enumerator => this;
        }

        public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerable<KeyValuePair<string, JSONNode>>
        {
            private JSONNode m_Node;
            private Enumerator m_Enumerator;
            internal LinqEnumerator(JSONNode aNode)
            {
                m_Node = aNode;
                if (m_Node != null)
                {
                    m_Enumerator = m_Node.GetEnumerator();
                }
            }
            public KeyValuePair<string, JSONNode> Current => m_Enumerator.Current;
            object IEnumerator.Current => m_Enumerator.Current;
            public bool MoveNext() { return m_Enumerator.MoveNext(); }

            public void Dispose()
            {
                m_Node = null;
                m_Enumerator = new Enumerator();
            }

            public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator() => new LinqEnumerator(m_Node);

            public void Reset()
            {
                if (m_Node != null)
                {
                    m_Enumerator = m_Node.GetEnumerator();
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => new LinqEnumerator(m_Node);
        }

        #endregion Enumerators

        #region common interface

        public static bool forceASCII = false; // Use Unicode by default
        public static bool longAsString = false; // lazy creator creates a JSONString instead of JSONNumber
        public static bool allowLineComments = true; // allow "//"-style comments at the end of a line

        public abstract Enums.JSONNodeType Tag { get; }

        public virtual JSONNode this[int aIndex] { get => null; set { } }

        public virtual JSONNode this[string aKey] { get => null; set { } }

        public virtual string Value { get => ""; set { } }
        public virtual int Count => 0;

        public virtual bool IsNumber => false;
        public virtual bool IsString => false;
        public virtual bool IsBoolean => false;
        public virtual bool IsNull => false;
        public virtual bool IsArray => false;
        public virtual bool IsObject => false;
        public virtual bool Inline { get => false; set { } }

        public virtual void Add(string aKey, JSONNode aItem)
        {
        }
        public virtual void Add(JSONNode aItem)
        {
            Add("", aItem);
        }

        public virtual JSONNode Remove(string aKey) => null;
        public virtual JSONNode Remove(int aIndex) => null;
        public virtual JSONNode Remove(JSONNode aNode) => aNode;
        public virtual JSONNode Clone() => null;

        public virtual void Clear() { }
        public virtual IEnumerable<JSONNode> Children
        {
            get
            {
                yield break;
            }
        }
        public IEnumerable<JSONNode> DeepChildren => Children.SelectMany(C => C.DeepChildren);
        public virtual bool HasKey(string aKey) => false;
        public virtual JSONNode GetValueOrDefault(string aKey, JSONNode aDefault) => aDefault;
        public override string ToString()
        {
            StringBuilder sb = new();
            WriteToStringBuilder(sb, 0, 0, Enums.JSONTextMode.Compact);
            return sb?.ToString();
        }
        public virtual string ToString(int aIndent)
        {
            StringBuilder sb = new();
            WriteToStringBuilder(sb, 0, aIndent, Enums.JSONTextMode.Indent);
            return sb?.ToString();
        }
        internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, Enums.JSONTextMode aMode);

        public abstract Enumerator GetEnumerator();
        public IEnumerable<KeyValuePair<string, JSONNode>> Linq => new LinqEnumerator(this);
        public KeyEnumerator Keys => new(GetEnumerator());
        public ValueEnumerator Values => new(GetEnumerator());

        #endregion common interface

        #region typecasting properties


        public virtual double AsDouble
        {
            get => double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double v) ? v : 0.0;
            set => Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public virtual int AsInt
        {
            get => (int)AsDouble;
            set => AsDouble = value;
        }

        public virtual float AsFloat
        {
            get => (float)AsDouble;
            set => AsDouble = value;
        }

        public virtual bool AsBool
        {
            get => bool.TryParse(Value, out bool v) ? v : !string.IsNullOrEmpty(Value);
            set => Value = value ? "true" : "false";
        }

        public virtual long AsLong
        {
            get => long.TryParse(Value, out long val) ? val : 0;
            set => Value = value.ToString();
        }

        public virtual ulong AsULong
        {
            get => ulong.TryParse(Value, out ulong val) ? val : 0;
            set => Value = value.ToString();
        }

        public virtual JSONArray AsArray => this as JSONArray;
        public virtual JSONObject AsObject => this as JSONObject;

        #endregion typecasting properties

        #region operators

        public static implicit operator JSONNode(string s) => (s == null) ? JSONNull.CreateOrGet : new JSONString(s);
        public static implicit operator string(JSONNode d) => d?.Value;
        public static implicit operator JSONNode(double n) => new JSONNumber(n);
        public static implicit operator double(JSONNode d) => (d == null) ? 0 : d.AsDouble;
        public static implicit operator JSONNode(float n) => new JSONNumber(n);
        public static implicit operator float(JSONNode d) => (d == null) ? 0 : d.AsFloat;
        public static implicit operator JSONNode(int n) => new JSONNumber(n);
        public static implicit operator int(JSONNode d) => (d == null) ? 0 : d.AsInt;
        public static implicit operator JSONNode(long n) => longAsString ? new JSONString(n.ToString()) : new JSONNumber(n);
        public static implicit operator long(JSONNode d) => (d == null) ? 0 : d.AsLong;
        public static implicit operator JSONNode(ulong n) => longAsString ? new JSONString(n.ToString()) : new JSONNumber(n);
        public static implicit operator ulong(JSONNode d) => (d == null) ? 0 : d.AsULong;
        public static implicit operator JSONNode(bool b) => new JSONBool(b);
        public static implicit operator bool(JSONNode d) => d != null && d.AsBool;
        public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue) => aKeyValue.Value;
        public static bool operator ==(JSONNode a, object b)
        {
            if (!ReferenceEquals(a, b))
            {
                bool aIsNull = a is JSONNull or null or JSONLazyCreator;
                return (aIsNull && b is JSONNull or null or JSONLazyCreator) || (!aIsNull && a.Equals(b));
            }
            return true;
        }
        public static bool operator !=(JSONNode a, object b) => !(a == b);
        public override bool Equals(object obj) => ReferenceEquals(this, obj);
       
        //public override bool Equals(object obj) => obj is bool boolean && this == boolean;

        public bool Equals(bool obj) => this == obj;
        public override int GetHashCode() => base.GetHashCode();

        #endregion operators

        [ThreadStatic]
        private static StringBuilder m_EscapeBuilder;
        internal static StringBuilder EscapeBuilder => m_EscapeBuilder ??= new StringBuilder();

        internal static string Escape(string aText)
        {
            StringBuilder sb = EscapeBuilder;
            sb.Length = 0;
            if (sb.Capacity < aText.Length + (aText.Length / 10))
            {
                sb.Capacity = aText.Length + aText.Length / 10;
            }

            foreach (char c in aText)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '\"': sb.Append("\\\""); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default:
                        if (c < ' ' || (forceASCII && c > 127))
                        {
                            ushort val = c;
                            sb.Append("\\u").Append(val.ToString("X4"));
                        }
                        else { sb.Append(c); } break;
                }
            }
            sb.Length = 0;
            return sb?.ToString();
        }

        private static JSONNode ParseElement(string token, bool quoted)
        {
            if (!quoted)
            {
                if (token.Length <= 5)
                {
                    string tmp = token.ToLower();
                    switch (tmp)
                    {
                        case "false" or "true": return tmp == "true";
                        case "null": return JSONNull.CreateOrGet;
                        default: break;
                    }
                }
                return double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double val) ? val : (JSONNode)token;
            }
            return token;
        }

        public static JSONNode Parse(string aJSON)
        {
            Stack<JSONNode> stack = new();
            JSONNode ctx = null;
            int i = 0;
            StringBuilder Token = new();
            string TokenName = "";
            bool QuoteMode = false, TokenIsQuoted = false, HasNewlineChar = false;
            while (i < aJSON.Length)
            {
                switch (aJSON[i])
                {
                    case '{':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }
                        stack.Push(new JSONObject());
                        if (ctx != null)
                        {
                            ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token.Length = 0;
                        ctx = stack.Peek();
                        HasNewlineChar = false;
                        break;

                    case '[':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }

                        stack.Push(new JSONArray());
                        if (ctx != null)
                        {
                            ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token.Length = 0;
                        ctx = stack.Peek();
                        HasNewlineChar = false;
                        break;

                    case '}':
                    case ']':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }
                        if (stack.Count != 0)
                        {
                            stack.Pop();
                            if (Token.Length > 0 || TokenIsQuoted)
                            {
                                ctx.Add(TokenName, ParseElement(Token?.ToString(), TokenIsQuoted));
                            }
                            if (ctx != null)
                            {
                                ctx.Inline = !HasNewlineChar;
                            }

                            TokenIsQuoted = false;
                            TokenName = "";
                            Token.Length = 0;
                            if (stack.Count > 0)
                            {
                                ctx = stack.Peek();
                            }
                            break;
                        }
                        throw new Exception("JSON Parse: Too many closing brackets");

                    case ':':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }
                        TokenName = Token.ToString();
                        Token.Length = 0;
                        TokenIsQuoted = false;
                        break;

                    case '"':
                        QuoteMode ^= true;
                        TokenIsQuoted |= QuoteMode;
                        break;

                    case ',':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }
                        if (Token.Length > 0 || TokenIsQuoted)
                        {
                            ctx.Add(TokenName, ParseElement(Token.ToString(), TokenIsQuoted));
                        }
                        TokenName = "";
                        Token.Length = 0;
                        TokenIsQuoted = false;
                        break;

                    case '\r':
                    case '\n':
                        HasNewlineChar = true;
                        break;
                    case ' ':
                    case '\t':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                        }

                        break;

                    case '\\':
                        ++i;
                        if (QuoteMode)
                        {
                            char C = aJSON[i];
                            switch (C)
                            {
                                case 't': Token.Append('\t'); break;
                                case 'r': Token.Append('\r'); break;
                                case 'n': Token.Append('\n'); break;
                                case 'b': Token.Append('\b'); break;
                                case 'f': Token.Append('\f'); break;
                                case 'u':
                                    {
                                        Token.Append((char)int.Parse(aJSON.Substring(i + 1, 4), NumberStyles.AllowHexSpecifier));
                                        i += 4;
                                        break;
                                    }
                                default: Token.Append(C); break;
                            }
                        }
                        break;
                    case '/':
                        if (!allowLineComments || QuoteMode || i + 1 >= aJSON.Length || aJSON[i + 1] != '/')
                        {
                            Token.Append(aJSON[i]); break;
                        }
                        while (++i < aJSON.Length && aJSON[i] != '\n' && aJSON[i] != '\r') {; }
                        break;
                    case '\uFEFF': break; // remove / ignore BOM (Byte Order Mark)
                    default: Token.Append(aJSON[i]); break;
                }
                ++i;
            }
            return QuoteMode ? throw new Exception("JSON Parse: Quotation marks seems to be messed up.") : ctx ?? ParseElement(Token.ToString(), TokenIsQuoted);
        }
    }
}
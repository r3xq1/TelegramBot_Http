namespace TelegramBotEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class JSONObject : JSONNode
    {
        private readonly Dictionary<string, JSONNode> m_Dict = new();

        private bool inline = false;
        public override bool Inline
        {
            get => inline;
            set => inline = value;
        }

        public override Enums.JSONNodeType Tag => Enums.JSONNodeType.Object;
        public override bool IsObject => true;

        public override Enumerator GetEnumerator() => new(m_Dict.GetEnumerator());
        public override JSONNode this[string aKey]
        {
            get => m_Dict.ContainsKey(aKey) ? m_Dict[aKey] : new JSONLazyCreator(this, aKey);
            set
            {
                if (value == null)
                {
                    value = JSONNull.CreateOrGet;
                }
                if (m_Dict.ContainsKey(aKey))
                {
                    m_Dict[aKey] = value;
                }
                else
                {
                    m_Dict.Add(aKey, value);
                }
            }
        }
        public override JSONNode this[int aIndex]
        {
            get => aIndex < 0 || aIndex >= m_Dict.Count ? null : m_Dict.ElementAt(aIndex).Value;
            set
            {
                if (value == null)
                {
                    value = JSONNull.CreateOrGet;
                }
                if (aIndex >= 0 && aIndex < m_Dict.Count)
                {
                    m_Dict[m_Dict.ElementAt(aIndex).Key] = value;
                }
            }
        }

        public override int Count => m_Dict.Count;

        public override void Add(string aKey, JSONNode aItem)
        {
            if (aItem == null)
            {
                aItem = JSONNull.CreateOrGet;
            }

            if (aKey != null)
            {
                if (m_Dict.ContainsKey(aKey))
                {
                    m_Dict[aKey] = aItem;
                }
                else
                {
                    m_Dict.Add(aKey, aItem);
                }
            }
            else
            {
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
            }
        }

        public override JSONNode Remove(string aKey)
        {
            if (m_Dict.ContainsKey(aKey))
            {
                JSONNode tmp = m_Dict[aKey];
                bool result = m_Dict.Remove(aKey);
                return tmp;
            }
            return null;
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
            {
                return null;
            }

            KeyValuePair<string, JSONNode> item = m_Dict.ElementAt(aIndex);
            bool result = m_Dict.Remove(item.Key);
            return item.Value;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            try
            {
                KeyValuePair<string, JSONNode> item = m_Dict.First(k => k.Value == aNode);
                bool result = m_Dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        public override void Clear()
        {
            m_Dict.Clear();
        }

        public override JSONNode Clone()
        {
            JSONObject node = new();
            foreach (KeyValuePair<string, JSONNode> n in m_Dict)
            {
                node.Add(n.Key, n.Value.Clone());
            }
            return node;
        }

        public override bool HasKey(string aKey) => m_Dict.ContainsKey(aKey);

        public override JSONNode GetValueOrDefault(string aKey, JSONNode aDefault) => m_Dict.TryGetValue(aKey, out JSONNode res) ? res : aDefault;

        public override IEnumerable<JSONNode> Children => m_Dict.Select(N => N.Value);

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, Enums.JSONTextMode aMode)
        {
            aSB.Append('{');
            bool first = true;
            if (inline)
            {
                aMode = Enums.JSONTextMode.Compact;
            }

            foreach (KeyValuePair<string, JSONNode> k in m_Dict)
            {
                if (!first)
                {
                    aSB.Append(',');
                }

                first = false;
                if (aMode == Enums.JSONTextMode.Indent)
                {
                    aSB.AppendLine();
                }
                if (aMode == Enums.JSONTextMode.Indent)
                {
                    aSB.Append(' ', aIndent + aIndentInc);
                }

                aSB.Append('\"').Append(Escape(k.Key)).Append('\"');
                aSB.Append(aMode == Enums.JSONTextMode.Compact ? ':' : " : ");

                k.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == Enums.JSONTextMode.Indent)
            {
                aSB.AppendLine().Append(' ', aIndent);
            }

            aSB.Append('}');
        }
    }
}
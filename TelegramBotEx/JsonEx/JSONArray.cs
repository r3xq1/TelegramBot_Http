namespace TelegramBotEx
{
    using System.Collections.Generic;
    using System.Text;

    public partial class JSONArray : JSONNode
    {
        private readonly List<JSONNode> m_List = new();
        private bool inline = false;
        public override bool Inline
        {
            get => inline;
            set => inline = value;
        }
        public override Enums.JSONNodeType Tag => Enums.JSONNodeType.Array;
        public override bool IsArray => true;
        public override Enumerator GetEnumerator() => new(m_List.GetEnumerator());
        public override JSONNode this[int aIndex]
        {
            get => aIndex < 0 || aIndex >= m_List.Count ? new JSONLazyCreator(this) : m_List[aIndex];
            set
            {
                if (value == null) { value = JSONNull.CreateOrGet;}
                if (aIndex < 0 || aIndex >= m_List.Count) { m_List.Add(value); }
                else { m_List[aIndex] = value; }
            }
        }
        public override JSONNode this[string aKey]
        {
            get => new JSONLazyCreator(this);
            set
            {
                if (value == null) { value = JSONNull.CreateOrGet; }
                m_List.Add(value);
            }
        }
        public override int Count => m_List.Count;
        public override void Add(string aKey, JSONNode aItem)
        {
            if (aItem == null) { aItem = JSONNull.CreateOrGet;}
            m_List.Add(aItem);
        }
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex >= 0 && aIndex < m_List.Count)
            {
                JSONNode tmp = m_List[aIndex];
                m_List.RemoveAt(aIndex);
                return tmp;
            }
            return null;
        }
        public override JSONNode Remove(JSONNode aNode)
        {
            bool result = m_List.Remove(aNode);
            return aNode;
        }
        public override void Clear() => m_List.Clear();
        public override JSONNode Clone()
        {
            JSONArray node = new();
            node.m_List.Capacity = m_List.Capacity;
            foreach (JSONNode n in m_List)
            {
                node.Add(n?.Clone());
            }
            return node;
        }
        public override IEnumerable<JSONNode> Children => m_List;
        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, Enums.JSONTextMode aMode)
        {
            aSB.Append('[');
            int count = m_List.Count;
            if (inline)
            {
                aMode = Enums.JSONTextMode.Compact;
            }

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    aSB.Append(',');
                }

                if (aMode == Enums.JSONTextMode.Indent)
                {
                    aSB.AppendLine();
                }

                if (aMode == Enums.JSONTextMode.Indent)
                {
                    aSB.Append(' ', aIndent + aIndentInc);
                }

                m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == Enums.JSONTextMode.Indent)
            {
                aSB.AppendLine().Append(' ', aIndent);
            }

            aSB.Append(']');
        }
    }
}
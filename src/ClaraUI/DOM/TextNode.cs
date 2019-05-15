/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Delta;

namespace Integrative.Clara.DOM
{
    public sealed class TextNode : Node
    {
        public override NodeType NodeType => NodeType.Text;

        string _data;

        public string Data
        {
            get => _data;
            set
            {
                if (_data != value)
                {
                    _data = value;
                    TextModifiedDelta.Enqueue(this);
                }
            }
        }

        public TextNode() : base(null)
        {
        }

        public TextNode(string data): base(null)
        {
            _data = data;
        }

        internal TextNode(Document document) : base(document)
        {
        }

        internal override ContentNode GetContentNode()
        {
            return new ContentTextNode
            {
                Data = _data,
            };
        }
    }
}

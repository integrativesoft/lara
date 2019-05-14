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

        string _text;

        public string Data
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    TextModifiedDelta.Enqueue(this);
                }
            }
        }

        public TextNode() : base(null)
        {
        }

        internal TextNode(Document document) : base(document)
        {
        }

        internal override ContentNode GetContentNode()
        {
            return new ContentTextNode
            {
                Data = _text,
            };
        }
    }
}

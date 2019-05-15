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

        public TextNode()
        {
        }

        public TextNode(string data)
        {
            _data = data;
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

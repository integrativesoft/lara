using System;
using System.ComponentModel;

namespace Integrative.Lara
{
    /// <summary>
    /// Conditional render placeholder
    /// </summary>
    public class RenderIf : WebComponent
    {
        private readonly Func<bool> _criteria;
        private readonly Func<Element> _factory;
        private bool _rendered;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="criteria"></param>
        /// <param name="factory"></param>
        public RenderIf(INotifyPropertyChanged source, Func<bool> criteria, Func<Element> factory)
        {
            _criteria = criteria;
            _factory = factory;
            source.PropertyChanged += (_, _) => Update();
        }

        private void Update()
        {
            var rendered = _criteria();
            if (rendered == _rendered) return;
            _rendered = rendered;
            if (rendered)
            {
                ShadowRoot.AppendChild(_factory());
            }
            else
            {
                ShadowRoot.ClearChildren();
            }
        }
    }
}

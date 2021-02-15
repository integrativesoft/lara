using Integrative.Lara;
using System.Collections.ObjectModel;

internal class MyList : WebComponent
{
    private readonly ObservableCollection<string> _names = new ObservableCollection<string>();

    public MyList()
    {
        ShadowRoot.Children = new Node[]
        {
            Fragment.ForEach(_names, (string name) => new HtmlDivElement { InnerText = name }),
        };
        _names.Add("Sarah");
        _names.Add("John");
    }
}

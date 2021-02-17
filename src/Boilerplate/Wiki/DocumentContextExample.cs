using Integrative.Lara;

internal class DocumentContextExample : WebComponent
{
    const string IconId = "MyIconElement";

    public DocumentContextExample()
    {
        ShadowRoot.Children = new Element[]
        {
            new HtmlDivElement
            {
                InnerText = "Check the title and icon of this webpage"
            }
        };
    }

    protected override void OnConnect() // our component is placed on Document
    {
        base.OnConnect();
        var icon = Document.GetElementById(IconId);
        if (icon != null) return;
        Document.Head.AppendChild(new HtmlLinkElement
        {
            Id = IconId,
            Rel = "icon",
            HRef = "https://stackoverflow.com/favicon.ico",
        });
        Document.Head.AppendChild(new HtmlTitleElement
        {
            InnerText = "Hello title",
        });
    }
}

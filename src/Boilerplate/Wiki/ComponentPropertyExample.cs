using Integrative.Lara;

internal class ComponentPropertyExample : WebComponent
{
    public ComponentPropertyExample()
    {
        var myLabel = new HtmlSpanElement
        {
            InnerText = "Hello!"
        };
        ShadowRoot.Children = new Element[]
        {
            new MyLabelComponent
            {
                Label = myLabel
            },
        };
    }
}

internal class MyLabelComponent : WebComponent
{
    private Element _label;
    public Element Label { get => _label; set => SetProperty(ref _label, value); }

    public MyLabelComponent()
    {
        ShadowRoot.Children = new Element[]
        {
            new RenderIf(this, () => _label != null, () => _label)
        };
    }
}

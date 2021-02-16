using Integrative.Lara;

internal class RedBoxExample : WebComponent
{
    public RedBoxExample()
    {
        ShadowRoot.Children = new Element[]
        {
            new RedBoxComponent
            {
                Children = new Element[]
                {
                    new HtmlDivElement
                    {
                        InnerText = "Hello!",
                    }
                }
            }
        };
    }
}

internal class RedBoxComponent : WebComponent
{
    public RedBoxComponent()
    {
        ShadowRoot.Children = new Element[]
        {
            new HtmlDivElement
            {
                Style = "border: solid 3px red",
                Children = new Element[]
                {
                    new Slot(),
                }
            }
        };
    }
}

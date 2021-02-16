using Integrative.Lara;

internal class ComposingComponent : WebComponent
{
    public ComposingComponent()  // parent component
    {
        ShadowRoot.Children = new Element[]
        {
            new ItemComponent { Name = "Sara" },
            new ItemComponent { Name = "Mike" },
            new ItemComponent { Name = "Tom" },
        };
    }
}

internal class ItemComponent : WebComponent  // child component
{
    private string _name;
    public string Name { get => _name; set => SetProperty(ref _name, value); }

    public ItemComponent()
    {
        ShadowRoot.Children = new Element[]
        {
            new HtmlDivElement
            {
                Children = new Element[]
                {
                    new HtmlTableCellElement()
                        .Bind(this, x => x.InnerText = Name),
                }
            }
        };
    }
}
using Integrative.Lara;

internal class UserTextComponent : WebComponent
{
    private string _name;
    public string Name { get => _name; set => SetProperty(ref _name, value); }

    public UserTextComponent()
    {
        Name = "Taylor";
        ShadowRoot.Children = new Element[]
        {
            new HtmlDivElement
                { InnerText = "Please enter your name: " },
            new HtmlInputElement()
                .Bind(this, x => x.Value = Name)  // if property changes, update element
                .BindBack(x => Name = x.Value),   // if element changes, update property
            new HtmlButtonElement
                { InnerText = "Read" }
                .Event("click", () => { }),
            new HtmlDivElement()
                .Bind(this, x => x.InnerText = $"Your name is {Name}"),
        };
    }
}

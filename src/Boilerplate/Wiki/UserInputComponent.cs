using Integrative.Lara;

internal class UserInputComponent : WebComponent
{
    int _counter;
    public int Counter { get => _counter; set => SetProperty(ref _counter, value); }

    public UserInputComponent()
    {
        ShadowRoot.Children = new Element[]
        {
            new HtmlDivElement()
                .Bind(this, x => x.InnerText = Counter.ToString()),
            new HtmlButtonElement
                { InnerText = "Increase" }
                .Event("click", () => Counter++)
        };
    }
}

using Integrative.Lara;

internal class ConditionalRenderComponent : WebComponent
{
    private bool _showText;
    public bool ShowText { get => _showText; set => SetProperty(ref _showText, value); }

    public ConditionalRenderComponent()
    {
        ShadowRoot.Children = new Node[]
        {
            new HtmlDivElement
            {
                InnerText = "Hello!",
            }
            .Bind(this, x => x.Render = ShowText) // Render property here
        };
    }
}

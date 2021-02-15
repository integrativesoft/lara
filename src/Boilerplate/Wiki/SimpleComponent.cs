using Integrative.Lara;

internal class SimpleComponent : WebComponent
{
    private string _message;
    private string Message { get => _message; set => SetProperty(ref _message, value); }

    public SimpleComponent()
    {
        Message = "My Lara app";
        ShadowRoot.Children = new Node[]
        {
                new HtmlDivElement
                {
                    Children = new Node[]
                    {
                        new HtmlSpanElement()
                            .Bind(this, x => x.InnerText = Message)
                    }
                }
        };
    }
}
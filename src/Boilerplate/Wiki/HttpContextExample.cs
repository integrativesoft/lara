using Integrative.Lara;

internal class HttpContextExample : WebComponent
{
    private string _message;
    public string Message { get => _message; set => SetProperty(ref _message, value); }

    public HttpContextExample()
    {
        ShadowRoot.Children = new Element[]
        {
            new HtmlDivElement()
                .Bind(this, x => x.InnerText = Message)            
        };
    }

    protected override void OnConnect()
    {
        base.OnConnect();
        Message = $"Your IP is {LaraUI.Context.Http.Connection.RemoteIpAddress}";
    }
}

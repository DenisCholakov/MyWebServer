namespace MyWebServer.Server.Results.Views
{
    public interface IView
    {
        string ExecuteTemplate(object model, string user);
    }
}

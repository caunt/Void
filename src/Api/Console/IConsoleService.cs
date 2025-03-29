namespace Void.Proxy.Api.Console;

public interface IConsoleService
{
    public void PrepareRender();
    public void Render(bool newLine = false);
    public void Setup();
}

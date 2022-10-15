namespace Notes.Application.Common.Interfaces.Providers;

public interface IContextInfoProvider
{
    public string GetUserId();
    public string GetRoute();
}
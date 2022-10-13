namespace Notes.Application.Common.Interfaces;

public interface IContextInfoProvider
{
    public string GetUserId();
    public string GetRoute();
}
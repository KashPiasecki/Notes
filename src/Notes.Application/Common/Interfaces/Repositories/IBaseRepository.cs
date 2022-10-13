namespace Notes.Application.Common.Interfaces.Repositories;

public interface IBaseRepository
{
    public Task SaveChangesAsync();
}
namespace Hackathon.ApiService.Databases.DbContexts
{
  public interface IMaindbContext
  {
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
  }
}

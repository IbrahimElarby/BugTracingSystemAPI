





namespace BugProject.DL
{
    public interface IUnitOfWork
    {
        public IBugRepository BugRepository { get; }

        public IProjectRepository ProjectRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
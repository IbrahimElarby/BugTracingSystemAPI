namespace BugProject.DL
{
    public interface IBugRepository
    {
        public Task<List<Bug>> GetAll();

        public Task<Bug> GetById(Guid id);

        public void Add(Bug bug);
    }
}
namespace BugProject.DL
{
    public interface IProjectRepository
    {
        public Task<List<Project>> GetAll();    

        public Task<Project> GetById(Guid id);

        public void Add(Project project);  
    }
}
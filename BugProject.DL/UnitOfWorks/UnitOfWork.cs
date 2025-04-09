

namespace BugProject.DL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BugContext context;

        public IBugRepository BugRepository { get; }

        public IProjectRepository ProjectRepository { get; }



        public UnitOfWork(
            BugContext _Context,
            IProjectRepository _projectRepository,
            IBugRepository _bugRepository
          
            )
        {
            context = _Context;
            BugRepository = _bugRepository;
            ProjectRepository = _projectRepository;
            
        }


        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }


    }
}

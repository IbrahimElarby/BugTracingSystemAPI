using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.DL
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly BugContext bugContext;

        public ProjectRepository(BugContext _bugContext)
        {
            bugContext = _bugContext;
        }
        public async Task<List<Project>> GetAll()
        {
            return await bugContext.Set<Project>()
                .Include(p => p.Bugs)
                .ThenInclude(b => b.Attachments)

                .Include(p => p.Bugs)
                .ThenInclude(b => b.Assignees)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Project> GetById(Guid id)
        {
            return await bugContext.Set<Project>()
                .Include(p => p.Bugs)
                .ThenInclude(b => b.Attachments)

                .Include(p => p.Bugs)
                .ThenInclude(b => b.Assignees)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p=>p.Id == id);
        }
        public async void Add(Project project)
        {
             await bugContext.Set<Project>().AddAsync(project);
        }
    }
}

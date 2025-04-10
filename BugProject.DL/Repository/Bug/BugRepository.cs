using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.DL
{
    public class BugRepository : IBugRepository
    {
        private readonly BugContext bugContext;

        public BugRepository(BugContext _bugContext)
        {
            bugContext = _bugContext;
        }
        public async Task<List<Bug>> GetAll()
        {
            return await bugContext.Set<Bug>()
                .Include(b => b.Assignees)
                .Include(b => b.Attachments)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Bug> GetById(Guid id)
        {
            return await bugContext.Set<Bug>()
              .Include(b => b.Assignees)
              .Include(b => b.Attachments)
              .AsSplitQuery()
              .AsNoTracking()
              .FirstOrDefaultAsync(b=>b.Id == id);
        }
        public async void Add(Bug bug)
        {
            await bugContext.Set<Bug>().AddAsync(bug);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await bugContext.Users.FindAsync(userId);
        }



        public async Task<bool> AssignUserAsync(Guid bugId, Guid userId)
        {
            var bug = await GetById (bugId);
            var user = await GetUserByIdAsync(userId);
            if (bug == null || user == null || bug.Assignees.Any(u => u.Id == userId)) return false;

            bug.Assignees.Add(user);
            return true;
        }

        public async Task<bool> UnassignUserAsync(Guid bugId, Guid userId)
        {
            var bug = await GetById(bugId);
            if (bug == null) return false;

            var user = bug.Assignees.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            bug.Assignees.Remove(user);
            return true;
        }
    }
}

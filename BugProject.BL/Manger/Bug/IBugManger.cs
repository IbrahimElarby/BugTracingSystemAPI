using BugProject.DL;

namespace BugProject.BL
{
    public interface IBugManger
    {
        public Task<List<BugReadDTO>> GetAllAsync();
       public  Task<BugReadDTO?> GetByIdAsync(Guid id);
        public Task<GeneralResult>  AddAsync(BugWriteDTO bug);
        public Task<GeneralResult> AssignUserAsync(Guid bugId, Guid userId);
        public Task<GeneralResult> UnassignUserAsync(Guid bugId, Guid userId);
    }
}
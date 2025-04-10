namespace BugProject.BL
{
    public interface IProjectManger
    {
        public Task<List<ProjectReadDTO>> GetAllAsync();
        public Task<ProjectReadDTO?> GetByIdAsync(Guid id);
        public Task<GeneralResult> AddAsync(ProjectWriteDTO item);
    }
}
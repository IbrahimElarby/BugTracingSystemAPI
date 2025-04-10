using BugProject.DL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.BL
{
    public class BugManger : IBugManger
    {
        private readonly IUnitOfWork unitOfWork;

        public BugManger(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<List<BugReadDTO>> GetAllAsync()
        {
           var bugs =  await unitOfWork.BugRepository.GetAll();

            if (bugs == null)
            {
                return null;
            }
            return bugs.Select(b => new BugReadDTO
            {
                Id = b.Id,
                ProjectId = b.ProjectId,
                Description = b.Description,
                Attachments = b.Attachments.Select(a=> new AttachmentReadDTO
                { Id = a.Id,
                FileName = a.FileName,
                FilePath = a.FilePath}).ToList(),
                AssigneeUsernames = b.Assignees.Select(a=>a?.UserName ?? "unAssigned").ToList(),
            }).ToList();
        }

        public async Task<BugReadDTO?> GetByIdAsync(Guid id)
        {
            var bug = await unitOfWork.BugRepository.GetById(id);
            if (bug == null)
            {
                return null;
            }
            return new BugReadDTO
            {
                Id = bug.Id,
                ProjectId = bug.ProjectId,
                Description = bug.Description,
                Attachments = bug.Attachments.Select(a => new AttachmentReadDTO
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FilePath = a.FilePath
                }).ToList(),
                AssigneeUsernames = bug.Assignees.Select(a => a?.UserName ?? "unAssigned").ToList(),
            };

        }
        public async Task<GeneralResult> AddAsync(BugWriteDTO item)
        {
            try
            {
                // Validate input
                if (item == null)
                {
                    return new GeneralResult
                    {
                        Success = false,
                        Errors = [new ResultError { Code = "NullInput", Message = "Bug cannot be null" }]
                    };
                }
                var bug = new Bug
                {
                    Id = item.BugId,
                    ProjectId = item.ProjectId,
                    Description = item.Description
                };
                unitOfWork.BugRepository.Add(bug);
                var saveResult = await unitOfWork.SaveChangesAsync();

                return saveResult > 0
                    ? new GeneralResult { Success = true }
                    : new GeneralResult { Success = false, Errors = [new ResultError { Code = "SaveFailed", Message = "No changes persisted" }] };
            }
            catch (DbUpdateException ex)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = [new ResultError
            {
                Code = "DatabaseError",
                Message = $"Failed to save Bug: {ex.InnerException?.Message ?? ex.Message}"
            }]
                };
            }
            catch (Exception ex)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = [new ResultError
            {
                Code = "AddFailed",
                Message = $"Unexpected error: {ex.Message}"
            }]
                };
            }
        }

        public async Task<GeneralResult> AssignUserAsync(Guid bugId, Guid userId)
        {
            var success = await unitOfWork.BugRepository.AssignUserAsync(bugId, userId);
            if (!success)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = new[]
                    {
                    new ResultError { Code = "AssignFailed", Message = "Could not assign user to bug" }
                }
                };
            }

            await unitOfWork.SaveChangesAsync();
            return new GeneralResult { Success = true };
        }


        public async Task<GeneralResult> UnassignUserAsync(Guid bugId, Guid userId)
        {
            var success = await unitOfWork.BugRepository.UnassignUserAsync(bugId, userId);
            if (!success)
            {
                return new GeneralResult
                {
                    Success = false,
                    Errors = new[]
                    {
                    new ResultError { Code = "UnassignFailed", Message = "Could not unassign user from bug" }
                }
                };
            }

            await unitOfWork.SaveChangesAsync();
            return new GeneralResult { Success = true };
        }
    }
}

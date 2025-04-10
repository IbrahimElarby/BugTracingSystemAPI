using BugProject.DL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Attachment = BugProject.DL.Attachment;

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

        public async Task<List<AttachmentReadDTO>> GetAttachment(Guid id)
        {
            var bug = await unitOfWork.BugRepository.GetById(id);
            if (bug == null)
            {
                return null;
            }

            return bug.Attachments.Select(a=>  new AttachmentReadDTO
            { FileName = a.FileName,
            FilePath = a.FilePath,
            Id = a.Id}).ToList();
        }

        public async Task<GeneralResult> AddAtatchment(Guid bugId, AttachmentReadDTO attachment)
        {
            try
            {
                var bug = await unitOfWork.BugRepository.GetById(bugId);
                if (bug == null)
                {
                    return new GeneralResult
                    {
                        Success = false,
                        Errors = [new ResultError { Code = "NullInput", Message = "bug cannot be null" }]
                    };
                }
                if (attachment == null)
                {
                    return new GeneralResult
                    {
                        Success = false,
                        Errors = [new ResultError { Code = "NullInput", Message = "attachment cannot be null" }]
                    };
                }
                var attach = new Attachment
                {
                    FileName = attachment.FileName,
                    FilePath = attachment.FilePath,
                    Id = attachment.Id,
                };
                bug.Attachments.Add(attach);
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

        public async Task<GeneralResult> DeleteAttachment(Guid bugId, Guid attId)
        {

            try
            {
                var bug = await unitOfWork.BugRepository.GetById(bugId);
                if (bug == null)
                {
                    return new GeneralResult
                    {
                        Success = false,
                        Errors = [new ResultError { Code = "NullInput", Message = "bug cannot be null" }]
                    };
                }
             
                var attachment = bug.Attachments.FirstOrDefault(a=>a.Id == attId);
                if (attachment == null)
                {
                    return new GeneralResult
                    {
                        Success = false,
                        Errors = [new ResultError { Code = "NullInput", Message = "attachment cannot be null" }]
                    };
                }
                bug.Attachments.Remove(attachment);
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
    }
}

using BugProject.DL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.BL
{
    public class ProjectManger : IProjectManger
    {
        private readonly IUnitOfWork unitOfWork;

        public ProjectManger(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<List<ProjectReadDTO>> GetAllAsync()
        {
            var projects = await unitOfWork.ProjectRepository.GetAll();
            if (projects == null)
            {
                return null;
            }
            return projects.Select(p=> new ProjectReadDTO
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                Bugs = p.Bugs.Select(b=> new BugReadDTO
                {
                    Id=b.Id,
                    ProjectId=b.Id,
                    Description=b.Description,
                    Attachments = b.Attachments.Select(a=>new AttachmentReadDTO
                    { FileName = a.FileName,
                    FilePath =a.FilePath,
                    Id = a.Id}).ToList(),
                    AssigneeUsernames = b.Assignees.Select(a => a?.UserName ?? "unAssigned").ToList()
                }).ToList(),
            
            }).ToList();
        }

        public async Task<ProjectReadDTO?> GetByIdAsync(Guid id)
        {
            var project = await unitOfWork.ProjectRepository.GetById(id);
            if (project == null)
            {
                return null;
            }
            return new ProjectReadDTO
            {
                Id = project.Id,
                Name = project.Name,
                Location = project.Location,
                Bugs = project.Bugs.Select(b => new BugReadDTO
                {
                    Id = b.Id,
                    ProjectId = b.Id,
                    Description = b.Description,
                    Attachments = b.Attachments.Select(a => new AttachmentReadDTO
                    {
                        FileName = a.FileName,
                        FilePath = a.FilePath,
                        Id = a.Id
                    }).ToList(),
                    AssigneeUsernames = b.Assignees.Select(a => a?.UserName ?? "unAssigned").ToList()
                }).ToList(),

            };
        }
        public async Task<GeneralResult> AddAsync(ProjectWriteDTO item)
        {
            try
            {
                // Validate input
                if (item == null)
                {
                    return new GeneralResult
                    {
                        Success = false,
                        Errors = [new ResultError { Code = "NullInput", Message = "Project cannot be null" }]
                    };
                }
                var project = new Project
                {
                    Id = item.Id,
                    Name= item.Name,
                    Location= item.Location,
      
                };
                unitOfWork.ProjectRepository.Add(project);
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
                Message = $"Failed to save Project: {ex.InnerException?.Message ?? ex.Message}"
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

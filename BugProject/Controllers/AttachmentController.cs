using Microsoft.AspNetCore.Mvc;

using BugProject.BL;
using Microsoft.AspNetCore.Authorization;

namespace BugProject
{

    [ApiController]
    [Route("api/bugs/{bugId}/attachments")]
    public class AttachmentController : ControllerBase
    {
        private readonly IBugManger _attachmentManager;

        public AttachmentController(IBugManger attachmentManager)
        {
            _attachmentManager = attachmentManager;
        }

       
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = Constatnts.Policies.ForTester)]
        public async Task<IActionResult> Upload(Guid bugId, [FromForm] FileUploadDTO dto)
        {
            var file = dto.File;
            if (file == null || file.Length == 0)
            {
                return BadRequest(new GeneralResult
                {
                    Success = false,
                    Errors = [new ResultError { Code = "InvalidFile", Message = "No file was uploaded." }]
                });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var result = await _attachmentManager.AddAtatchment(bugId, new AttachmentReadDTO
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                FilePath = $"/uploads/{fileName}"
            });

            return result.Success ? Ok(result) : BadRequest(result);
        }


       
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetByBugId(Guid bugId)
        {
            var result = await _attachmentManager.GetAttachment(bugId);

            if (result == null || !result.Any())
            {
                return NotFound(new GeneralResult
                {
                    Success = false,
                    Errors = [new ResultError { Code = "NotFound", Message = "No attachments found for this bug." }]
                });
            }

            return Ok(new GeneralResult<List<AttachmentReadDTO>>
            {
                Success = true,
                Data = result
            });
        }

        
        [HttpDelete("{attachmentId}")]
        [Authorize(Policy = Constatnts.Policies.ForTester)]
        public async Task<IActionResult> Delete(Guid bugId, Guid attachmentId)
        {
            var result = await _attachmentManager.DeleteAttachment(bugId, attachmentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }

}

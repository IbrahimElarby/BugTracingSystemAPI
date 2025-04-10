using BugProject.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BugProject
{


    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectManger projectManger;

        public ProjectController(IProjectManger _projectManger)
        {
            projectManger = _projectManger;
        }

        [HttpGet]
        [Authorize]
        public async Task<Ok<List<ProjectReadDTO>>> GetAll()
        {
            return TypedResults.Ok(await projectManger.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<Results<Ok<ProjectReadDTO>, NotFound>> GetById(Guid id)
        {
            var project = await projectManger.GetByIdAsync(id);
            if (project == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(project);
        }

        [HttpPost]
        [Authorize(Policy = Constatnts.Policies.ForAdminOnly)]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> Add(ProjectWriteDTO project)
        {
            var result = await projectManger.AddAsync(project);
            if (result.Success)
            {
                return TypedResults.Ok(result);
            }
            return TypedResults.BadRequest(result);
        }
    }
}

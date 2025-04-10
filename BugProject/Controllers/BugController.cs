using BugProject.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BugProject
{
    [ApiController]
    [Route("api/[controller]")]
    public class BugController : ControllerBase
    {
        private readonly IBugManger bugManger;

        public BugController(IBugManger _bugManger)
        {
            bugManger = _bugManger;
        }

        [HttpGet]
        [Authorize]
        public async Task<Ok<List<BugReadDTO>>> GetAll()
        {
            return TypedResults.Ok(await bugManger.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<Results<Ok<BugReadDTO>,NotFound>> GetById( Guid id)
        {
            var bug =  await bugManger.GetByIdAsync(id);
            if (bug == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(bug);
        }

        [HttpPost]
        [Authorize(Policy = Constatnts.Policies.ForAdminOnly)]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> Add(BugWriteDTO bug)
        {
            var result = await bugManger.AddAsync(bug);
            if (result.Success)
            {
                return TypedResults.Ok(result);
            }
            return TypedResults.BadRequest(result);
        }
        [HttpPost("{id}/assignees")]
        [Authorize(Policy = Constatnts.Policies.ForAdminOnly)]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> AssignUser(Guid id, [FromBody] UserReadDTO dto)
        {
            var result = await bugManger.AssignUserAsync(id, dto.Id);

            if (result.Success)
            {
                return TypedResults.Ok(result);
            }
            return TypedResults.BadRequest(result);
        }

        [HttpDelete("{id}/assignees/{userId}")]
        [Authorize(Policy = Constatnts.Policies.ForAdminOnly)]
        public async Task<Results<Ok<GeneralResult>, BadRequest<GeneralResult>>> UnassignUser(Guid id, Guid userId)
        {
            var result = await bugManger.UnassignUserAsync(id, userId);

            if (result.Success)
            {
                return TypedResults.Ok(result);
            }
            return TypedResults.BadRequest(result);
        }
    }
}

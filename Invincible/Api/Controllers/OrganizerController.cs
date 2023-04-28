using Invincible.Api.RequestsBodies.Organizer;
using Invincible.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RebuildUkraineHackatonWebAPI.MongoDB.StatusCodes;

namespace RebuildUkraineHackatonWebAPI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/organizer")]
public class OrganizerController : Controller
{
    public OrganizerController(IMongoDBController mongo_db_controller)
        : base(mongo_db_controller){}

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> updateOrganizer(PutOrganizerRequestBody request_body)
    {
        if (!tryGetGoogleId(out string google_id))
            return BadRequest();

        if (await _db_controller.updateOrganizerInfo(google_id, request_body) == UpdateStatusCode.NOT_FOUND)
            return NotFound();
        else
            return Ok();
    }
}

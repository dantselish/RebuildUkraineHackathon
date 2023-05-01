using Invincible.Api.RequestsBodies.Volunteer;
using Invincible.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RebuildUkraineHackatonWebAPI.MongoDB.StatusCodes;

namespace RebuildUkraineHackatonWebAPI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/volunteer")]
public class VolunteerController : MyController
{
    public VolunteerController(IMongoDBController mongo_db_controller)
        : base(mongo_db_controller){}

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> updateVolunteer(PutVolunteerRequestBody request_body)
    {
        if (!tryGetGoogleId(out string google_id))
            return BadRequest();

        if (await _db_controller.updateVolunteerInfo(google_id, request_body) == UpdateStatusCode.NOT_FOUND)
            return NotFound();

        return Ok();
    }
}

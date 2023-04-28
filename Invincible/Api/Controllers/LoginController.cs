using Invincible.Api.RequestsBodies.Auth;
using Invincible.Items;
using Invincible.MongoDB;
using Invincible.Responces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RebuildUkraineHackatonWebAPI.MongoDB;

namespace RebuildUkraineHackatonWebAPI.Api.Controllers;

[ApiController]
[Route("login")]
public class LoginController : Controller
{
    public LoginController(IMongoDBController mongo_db_controller) : base(mongo_db_controller)
    {
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LoginResponse>> login(LoginRequestBody request_body)
    {
        if (!tryGetGoogleId(out string google_id))
            return BadRequest();

        if (request_body.isOrganizer)
        {
            OrganizerItem? organizer = await _db_controller.getOrganizer(google_id);
            if (organizer != null)
                return new LoginResponse {isNewUser = false};

            await _db_controller.createOrganizer(new OrganizerCreationData(){ googleId = google_id});
        }
        else
        {
            VolunteerItem? volunteer = await _db_controller.getVolunteer(google_id);
            if (volunteer != null)
                return new LoginResponse {isNewUser = false};

            await _db_controller.createVolunteer(new VolunteerCreationData(){ googleId = google_id});
        }

        return new LoginResponse{ isNewUser = true };
    }
}

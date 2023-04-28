using Invincible.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace RebuildUkraineHackatonWebAPI.Api.Controllers;

public class Controller : ControllerBase
{
    protected readonly IMongoDBController _db_controller;


    public Controller(IMongoDBController mongo_db_controller)
    {
        _db_controller = mongo_db_controller;
    }

    protected bool tryGetGoogleId( out string google_id )
    {
        google_id = String.Empty;
        if (!HttpContext.Request.HttpContext.User.HasClaim(x=> x.Type.Equals(JwtRegisteredClaimNames.Sub)))
            return false;

        google_id = HttpContext.Request.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;
        return true;
    }
}

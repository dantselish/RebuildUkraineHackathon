using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RebuildUkraineHackathonWebAPI.Responces;
using RebuildUkraineHackatonWebAPI.MongoDB;

namespace RebuildUkraineHackathonWebAPI.Controllers;

[ServiceFilter( typeof( BasicAuthServiceFilter ) )]
[ApiController]
[Route( "api/user" )]
public class ApiUserController : ControllerBase
{
  private readonly IMongoDBController _db_controller;


  public ApiUserController( IMongoDBController mongo_db_controller )
  {
    _db_controller = mongo_db_controller;
  }

  [HttpPost( "create" )]
  public async Task<IActionResult> createUser( UserRequestBody request_body )
  {
    await _db_controller.insertUser( request_body.user );
    return Ok();
  }

  [HttpGet( "all" )]
  public async Task<AllUsersResponse> getAllUsers()
  {
    return new AllUsersResponse(){ users = await _db_controller.getAllUsers() };
  }
}

public class UserRequestBody
{
  public User user { get; set; }
}
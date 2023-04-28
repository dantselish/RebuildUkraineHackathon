using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Invincible.MongoDB;
using Invincible.Responces;

namespace Invincible.Controllers;

[Authorize]
[ApiController]
[Route( "api/user" )]
public class ApiUserController : ControllerBase
{
  private readonly IMongoDBController _db_controller;


  public ApiUserController( IMongoDBController mongo_db_controller )
  {
    _db_controller = mongo_db_controller;
  }
}
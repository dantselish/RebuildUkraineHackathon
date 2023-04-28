using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Invincible.Controllers;

public class BasicAuthServiceFilter : ActionFilterAttribute
{
  public const string KEY_HEADER_NAME = "Authorization";

  private readonly string _auth_key = string.Empty;


  public BasicAuthServiceFilter( IConfiguration configuration )
  {
    _auth_key = configuration["AuthKey"];
  }

  public override void OnActionExecuting(ActionExecutingContext context)
  {
    if ( context.HttpContext.Request.Headers.TryGetValue( KEY_HEADER_NAME, out StringValues header_key ) )
    {
      if ( _auth_key.Equals( header_key ) )
      {
        base.OnActionExecuting( context );
        return;
      }
    }

    context.Result = new StatusCodeResult( StatusCodes.Status401Unauthorized );
  }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public class GoogleTokenValidator : ISecurityTokenValidator
{
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public GoogleTokenValidator()
    {
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public bool CanValidateToken => true;

    public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

    public bool CanReadToken(string securityToken)
    {
        return _tokenHandler.CanReadToken(securityToken);
    }

    public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        validatedToken = _tokenHandler.ReadJwtToken(securityToken);
        
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new []{ "780517687865-buts8ebadk6gqqlcje4ge1rvolcrtvig.apps.googleusercontent.com", "780517687865-2b0jpe448vqslb6152dsnejqhb5i10e7.apps.googleusercontent.com" }
        };
        var payload = GoogleJsonWebSignature.ValidateAsync(securityToken, settings).Result;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, payload.Name),
            new (ClaimTypes.Name, payload.Name),
            new (JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
            new (JwtRegisteredClaimNames.GivenName, payload.GivenName),
            new (JwtRegisteredClaimNames.Email, payload.Email),
            new (JwtRegisteredClaimNames.Sub, payload.Subject),
            new (JwtRegisteredClaimNames.Iss, payload.Issuer)
        };

        try
        {
            var principle = new ClaimsPrincipal();
            principle.AddIdentity(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
            return principle;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;

        }
    }
}

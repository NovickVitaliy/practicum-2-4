using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace Nimble.Modulith.Users.Endpoints;

public class Profile(SignInManager<IdentityUser> signInManager) : EndpointWithoutRequest<LogoutResponse>
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;

    public override void Configure()
    {
        Post("/profile");
        Tags("Account");
        AllowAnonymous(); // For now, we'll require auth later when we implement JWT properly
        Summary(s => {
            s.Summary = "Profile the current user";
            s.Description = "Gets the user's profile";
        });
    }
    
    public override async Task HandleAsync(CancellationToken ct)
    {
        Response = new LogoutResponse
        {
            Success = true,
            Message = "Profile of the current user"
        };
        
        await Send.OkAsync(Response, ct);
    }
}
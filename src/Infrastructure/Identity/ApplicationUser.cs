using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture8.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
}

    public class SignInUser
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

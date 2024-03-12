using Microsoft.AspNetCore.Identity;


public class AppUserViewModel
{

    public AppUserViewModel(ApplicationUser appUser, List<IdentityRole> roles)
    {
        this.ApprovedDevices = appUser.ApprovedDeviced;
        this.Email = appUser.Email;
        this.Roles = roles;
    }

    public List<string>? ApprovedDevices { get; set; }

    public string Email { get; set; }

    public List<IdentityRole>? Roles { get; set; }

}

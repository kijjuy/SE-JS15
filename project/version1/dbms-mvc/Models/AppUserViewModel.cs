using Microsoft.AspNetCore.Identity;


public class AppUserViewModel
{

    public AppUserViewModel(ApplicationUser appUser, List<string> roles)
    {
        this.ApprovedDevices = appUser.ApprovedDeviced;
        this.Email = appUser.Email;
        this.Roles = roles;
    }

    public List<string>? ApprovedDevices { get; set; }

    public string Email { get; set; }

    public List<string>? Roles { get; set; }

}

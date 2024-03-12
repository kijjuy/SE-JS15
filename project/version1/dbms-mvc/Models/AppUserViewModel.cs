using Microsoft.AspNetCore.Identity;


public class AppUserViewModel
{

    public AppUserViewModel(ApplicationUser appUser, IList<string> roles)
    {
        this.Id = appUser.Id;
        this.ApprovedDevices = appUser.ApprovedDeviced;
        this.Email = appUser.Email;
        this.Roles = roles;
    }

    public string Id { get; set; }

    public List<string>? ApprovedDevices { get; set; }

    public string Email { get; set; }

    public IList<string>? Roles { get; set; }

}

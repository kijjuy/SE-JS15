public class AppUserViewModel
{

    public AppUserViewModel(ApplicationUser appUser, IList<string> roles)
    {
        this.Id = appUser.Id;
        this.Email = appUser.Email;
        if (appUser.ApprovedDeviced != null)
        {
            this.ApprovedDevices = appUser.ApprovedDeviced;
        }
        else
        {
            this.ApprovedDevices = new List<string>();
        }
        if (roles != null)
        {
            this.Roles = roles;
        }
        else
        {
            this.Roles = new List<string>();
        }
    }

    public string Id { get; set; }

    public List<string>? ApprovedDevices { get; set; }

    public string Email { get; set; }

    public IList<string>? Roles { get; set; }

}

using Microsoft.AspNetCore.Identity;

public class ApplicationUser: IdentityUser {

    public List<string>? ApprovedDeviced { get; set; }  

}

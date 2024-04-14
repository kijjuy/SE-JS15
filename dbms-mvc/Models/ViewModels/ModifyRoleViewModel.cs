

public class ModifyRoleViewModel
{

    public ModifyRoleViewModel(ApplicationUser appUser, IList<string> roles, List<string> allRoles) {
        AppUserViewModel = new AppUserViewModel(appUser, roles);
        AllRoles = allRoles;
    }

    public AppUserViewModel AppUserViewModel { get; set; }

    public List<string> AllRoles { get; set; }

}

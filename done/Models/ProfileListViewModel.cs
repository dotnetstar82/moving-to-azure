namespace MovingToAzure.Models;

public class ProfileListViewModel
{
    public List<ProfileViewModel> Profiles { get; set; } = new List<ProfileViewModel>();
    public CrudStatus? StatusMessage { get; set; }
}

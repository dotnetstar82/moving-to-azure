namespace MovingToAzure.Models;

public class ProfileViewModel
{
    public bool Editing => Id > 0;
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? ProfilePic { get; set; }
}

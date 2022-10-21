using AutoMapper.Configuration.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MovingToAzure.Models;

public class ProfilePostModel
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = "";
    [StringLength(200)]
    public string? Address { get; set; }
    [StringLength(100)]
    public string? City { get; set; }
    [StringLength(2)]
    public string? State { get; set; }
    [StringLength(10)]
    public string? Zip { get; set; }
    [Ignore]
    public IFormFile? ProfilePic { get; set; }
}

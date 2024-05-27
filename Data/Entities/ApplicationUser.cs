
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ApplicationUser : IdentityUser
{

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Biography { get; set; }
    public string? ProfileImage { get; set; } = "avatar.jpg";
    public int? AddressId { get; set; }
    public AddressEntity? Address { get; set; }

}



public class AddressEntity
{
    [Key]
    public int Id { get; set; }
    public string AddressLine_1 { get; set; } = null!;
    public string? AddressLine_2 { get; set; }
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
}
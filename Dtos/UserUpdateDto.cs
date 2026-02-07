using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Dtos;

public class UserUpdateDto
{
    [Required, MinLength(2)]
    public string FirstName { get; set; } = "";

    [Required, MinLength(2)]
    public string LastName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, MinLength(2)]
    public string Department { get; set; } = "";

    public bool IsActive { get; set; } = true;
}

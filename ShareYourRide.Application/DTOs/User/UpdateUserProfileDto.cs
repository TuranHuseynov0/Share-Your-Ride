using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShareYourRide.Application.DTOs.User
{
    public class UpdateUserProfileDto
    {
        [MaxLength(100)] public string? FirstName { get; set; }
        [MaxLength(100)] public string? LastName { get; set; }
        [MaxLength(300)] public string? Bio { get; set; }
        [Phone] public string? PhoneNumber { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Auth
{
    public class RegisterVehicleDto
    {
        [Required] public Guid UserId { get; set; }

        [Required] public string Brand { get; set; } = default!;
        [Required] public string Model { get; set; } = default!;
        [Required] public string Color { get; set; } = default!;
        [Required] public int Year { get; set; }              // sən bu dəfə "il"-i də qeyd etdin, əvvəlki versiyada unutmuşdum, əlavə edirəm
        [Required] public string PlateNumber { get; set; } = default!;

        [Required] public string FrontImagePath { get; set; } = default!;
        [Required] public string BackImagePath { get; set; } = default!;
        [Required] public string LeftImagePath { get; set; } = default!;
        [Required] public string RightImagePath { get; set; } = default!;
    }
}

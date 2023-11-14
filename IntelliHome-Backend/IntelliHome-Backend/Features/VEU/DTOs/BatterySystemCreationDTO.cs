﻿using IntelliHome_Backend.Features.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace IntelliHome_Backend.Features.VEU.DTOs
{
    public class BatterySystemCreationDTO : SmartDeviceDTO
    {
        [Required(ErrorMessage = "Capacity is required.")]
        [Range(10, 1000, ErrorMessage = "Capacity should be between 10 and 1000 kWh")]
        public Double Capacity { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreCityInfo.Models
{
    public class PointOfInterestForUpdateDto
    {
        [Required(ErrorMessage ="You must provide a Point of Interest name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreCityInfo.Entities
{
    public class City
    {
        [Key] // è superfluo se la proprietà si chiama Id
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // è superfluo se la proprietà si chiama Id
        public int Id { get; set; } // Per convenzione, il solo fatto di chiamarsi Id  ed essere un intero, fa si
                                    // che sul db diventi un Identity

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public ICollection<PointOfInterest> PointsOfInterest { get; set; }
        = new List<PointOfInterest>();
    }
}

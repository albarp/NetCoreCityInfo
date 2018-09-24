using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreCityInfo.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]            
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // Aggiunto dopo come test per le migrazionie entity framework
        [MaxLength(200)]
        public string Description { get; set; }

        [ForeignKey("CityId")] // sarebbe superfluo
        public City City { get; set; } // per convenzione basterebbe questo a creare
                                       // una chiave esterna che punto al campo Id di City

        public int CityId{get;set;} // Sarebbe superfluo
    }
}

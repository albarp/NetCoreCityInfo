using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreCityInfo.Entities
{
    public class CityInfoContext : DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            :base(options)
        {
            // Così crea il db, ma un apparoccio migliore è utilizzare migrate, perchè
            // è in grado di costruire il db applicando tutte le migrazioni (la prima è la creazione del db stesso)
            // Database.EnsureCreated();
            // Le migrazioni si costruiscono con il comando Add-Migration "nome migrazione" dal lanciare nella
            // Package Manager Cconsole. Ogni volta il tool fa la differenza con le entity precedenti e crea
            // il file di migrazione
            Database.Migrate();
        }

        public DbSet<City> Cities { get; set; }

        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        // Funziona anche così
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("connectionstring");

        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}

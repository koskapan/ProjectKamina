using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamina.DAL.Models
{
    public class KaminaDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public  DbSet<Material> Materials { get; set; }

        public  DbSet<MaterialVersion> Versions { get; set; }

    }
}

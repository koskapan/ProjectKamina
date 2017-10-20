using Kamina.DAL.Models;

namespace Kamina.DAL.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Kamina.DAL.Models.KaminaDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Kamina.DAL.Models.KaminaDbContext context)
        {

            context.Categories.AddOrUpdate(c => c.Id, 
                new Category()
                {
                    Id = Guid.Parse("b12070ae-cccb-4e25-ad51-3d56d19b5eb1"),
                    Name = "Presentation"
                },
                new Category()
                {
                    Id = Guid.Parse("b12070ae-cccb-4e25-ad51-3d56d19b5eb2"),
                    Name = "Application"
                },
                new Category()
                {
                    Id = Guid.Parse("b12070ae-cccb-4e25-ad51-3d56d19b5eb3"),
                    Name = "Other"
                }
                );

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}

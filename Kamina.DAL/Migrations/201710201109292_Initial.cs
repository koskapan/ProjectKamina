namespace Kamina.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Materials",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        category_id = c.Guid(nullable: false),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.MaterialVersions",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        material_id = c.Guid(nullable: false),
                        version_number = c.Int(nullable: false),
                        version_size = c.Long(nullable: false),
                        create_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MaterialVersions");
            DropTable("dbo.Materials");
            DropTable("dbo.Categories");
        }
    }
}

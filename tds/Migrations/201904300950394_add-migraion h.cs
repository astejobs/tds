namespace tds.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmigraionh : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contractors",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        name = c.String(nullable: false),
                        GSTIN = c.String(nullable: false),
                        regNo = c.String(nullable: false),
                        phoneNo = c.Long(nullable: false),
                        address = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Deductors",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        legalName = c.String(nullable: false),
                        GSTIN = c.String(),
                        tradeName = c.String(),
                        departmentName = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Taxes",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        rate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        contractorId = c.String(maxLength: 128),
                        amountPaid = c.Double(nullable: false),
                        cgstId = c.String(maxLength: 128),
                        sgstId = c.String(maxLength: 128),
                        incometaxtId = c.String(),
                        labourCessId = c.String(maxLength: 128),
                        cgstAmount = c.Double(nullable: false),
                        sgstAmount = c.Double(nullable: false),
                        itAmount = c.Double(nullable: false),
                        incomeTax_id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Taxes", t => t.cgstId)
                .ForeignKey("dbo.Contractors", t => t.contractorId)
                .ForeignKey("dbo.Taxes", t => t.incomeTax_id)
                .ForeignKey("dbo.Taxes", t => t.labourCessId)
                .ForeignKey("dbo.Taxes", t => t.sgstId)
                .Index(t => t.contractorId)
                .Index(t => t.cgstId)
                .Index(t => t.sgstId)
                .Index(t => t.labourCessId)
                .Index(t => t.incomeTax_id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Transactions", "sgstId", "dbo.Taxes");
            DropForeignKey("dbo.Transactions", "labourCessId", "dbo.Taxes");
            DropForeignKey("dbo.Transactions", "incomeTax_id", "dbo.Taxes");
            DropForeignKey("dbo.Transactions", "contractorId", "dbo.Contractors");
            DropForeignKey("dbo.Transactions", "cgstId", "dbo.Taxes");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Transactions", new[] { "incomeTax_id" });
            DropIndex("dbo.Transactions", new[] { "labourCessId" });
            DropIndex("dbo.Transactions", new[] { "sgstId" });
            DropIndex("dbo.Transactions", new[] { "cgstId" });
            DropIndex("dbo.Transactions", new[] { "contractorId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Transactions");
            DropTable("dbo.Taxes");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Deductors");
            DropTable("dbo.Contractors");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasAPI.Migrations
{
    public partial class AdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT[AspNetRoles] ON;
            INSERT INTO[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES(N'fb9f0252-d6a2-4fa6-8720-951c59ea6162', N'dbccbbb1-9225-43ee-9795-afd56046b451', N'Admin', N'Admin');
            IF EXISTS(SELECT* FROM [sys].[identity_columns] WHERE[name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND[object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT[AspNetRoles] OFF;
            GO

            IF EXISTS(SELECT * FROM[sys].[identity_columns] WHERE[name] IN(N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND[object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT[AspNetUsers] ON;
            INSERT INTO[AspNetUsers] ([Id], [AccessFailedCount], [ConcurrencyStamp], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
VALUES(N'0779274b-f2fb-452e-b403-3e2cd3d770a6', 0, N'2397a327-a231-4fbe-a8a3-f8f0674c6a75', N'luis.gonzalez@computernet-gt.com', CAST(0 AS bit), CAST(0 AS bit), NULL, N'luis.gonzalez@computernet-gt.com', N'luis.gonzalez@computernet-gt.com', N'AQAAAAEAACcQAAAAEOf+mqFSVIDugkoUnsu8QkmgE0unNsAV3NttR0pSo26zxIeFGf+fzzcXJfSzDwZajA==', NULL, CAST(0 AS bit), N'fc8164c7-f803-4110-ac54-2c653d21c0a0', CAST(0 AS bit), N'luis.gonzalez@computernet-gt.com');
            IF EXISTS(SELECT* FROM [sys].[identity_columns] WHERE[name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND[object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT[AspNetUsers] OFF;
            GO

            IF EXISTS(SELECT * FROM[sys].[identity_columns] WHERE[name] IN(N'Id', N'ClaimType', N'ClaimValue', N'UserId') AND[object_id] = OBJECT_ID(N'[AspNetUserClaims]'))
    SET IDENTITY_INSERT[AspNetUserClaims] ON;
            INSERT INTO[AspNetUserClaims] ([Id], [ClaimType], [ClaimValue], [UserId])
VALUES(1, N'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', N'Admin', N'0779274b-f2fb-452e-b403-3e2cd3d770a6');
            IF EXISTS(SELECT* FROM [sys].[identity_columns] WHERE[name] IN (N'Id', N'ClaimType', N'ClaimValue', N'UserId') AND[object_id] = OBJECT_ID(N'[AspNetUserClaims]'))
    SET IDENTITY_INSERT[AspNetUserClaims] OFF;
            GO                

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "AspNetRoles",
               keyColumn: "Id",
               keyValue: "fb9f0252-d6a2-4fa6-8720-951c59ea6162");

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0779274b-f2fb-452e-b403-3e2cd3d770a6");

        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/*
 Entity’i ekledin ama Migration üretmedin veya DB’ye göndermedin ⇒ EF Core bu alanı hiç bilmiyor.

✔ Çözüm

Yeni migration çıkar:

Add-Migration AddPersonPhotoTable
Update-Database
 */
namespace RegionalSearch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonPhotoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "PersonPhotos");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "PersonPhotos");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "PersonPhotos");

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "PersonPhotos",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "PersonPhotos");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "PersonPhotos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "PersonPhotos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "PersonPhotos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

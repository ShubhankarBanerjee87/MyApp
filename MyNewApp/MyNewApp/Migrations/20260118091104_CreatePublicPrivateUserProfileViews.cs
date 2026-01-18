using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNewApp.Migrations
{
    /// <inheritdoc />
    public partial class CreatePublicPrivateUserProfileViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW vw_UserPrivateProfile
                AS
                SELECT
                    u.UserName,
                    u.Email,
                    u.ProfileImage,
                    ud.FirstName,
                    ud.LastName,
                    ud.DateOfBirth,
                    ud.Address,
                    ud.PhoneNumber
                FROM Users u
                INNER JOIN UserDetails ud ON ud.UserId = u.Id AND ud.IsActive = 1
                WHERE u.IsActive = 1
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW vw_UserPublicProfile
                AS 
                SELECT
                    u.UserName,
                    u.Email,
                    u.ProfileImage,
                    ud.FirstName,
                    ud.LastName,
                FROM Users u
                INNER JOIN UserDetails ud ON ud.UserId = u.Id AND ud.IsActive = 1
                WHERE u.IsActive = 1
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_UserPrivateProfile");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_UserPublicProfile");
        }
    }
}

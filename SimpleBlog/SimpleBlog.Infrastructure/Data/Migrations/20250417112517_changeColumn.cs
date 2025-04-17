using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleBlog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_LikeDisLikes_LikeDisLikeId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "LikeDisLikes");

            migrationBuilder.DropIndex(
                name: "IX_Posts_LikeDisLikeId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsReacted",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "LikeDisLikeId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "React",
                table: "Reactions",
                newName: "ReactType");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Posts",
                newName: "PostStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReactType",
                table: "Reactions",
                newName: "React");

            migrationBuilder.RenameColumn(
                name: "PostStatus",
                table: "Posts",
                newName: "Status");

            migrationBuilder.AddColumn<bool>(
                name: "IsReacted",
                table: "Reactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LikeDisLikeId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LikeDisLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisLike = table.Column<int>(type: "int", nullable: false),
                    Like = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeDisLikes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_LikeDisLikeId",
                table: "Posts",
                column: "LikeDisLikeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_LikeDisLikes_LikeDisLikeId",
                table: "Posts",
                column: "LikeDisLikeId",
                principalTable: "LikeDisLikes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

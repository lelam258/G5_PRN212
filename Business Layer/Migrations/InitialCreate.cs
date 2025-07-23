using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Business_Layer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
        name: "Students",
        columns: table => new
        {
            StudentId = table.Column<int>(nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            StudentCode = table.Column<string>(nullable: false),
            StudentName = table.Column<string>(nullable: false),
            Password = table.Column<string>(nullable: false),
            Email = table.Column<string>(nullable: true),
            Status = table.Column<string>(maxLength: 50, nullable: true),
            PhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
            DateOfBirth = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
            AvatarPath = table.Column<string>(nullable: true),
            LastLogin = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Students", x => x.StudentId);
        });

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    InstructorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorName = table.Column<string>(nullable: false),
                    Experience = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.InstructorId);
                });

            migrationBuilder.CreateTable(
                name: "LifeSkillCourses",
                columns: table => new
                {
                    CourseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseName = table.Column<string>(nullable: false),
                    InstructorId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    Description = table.Column<string>(nullable: true),
                    MaxStudents = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeSkillCourses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_LifeSkillCourses_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    AssessmentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(nullable: false),
                    AssessmentName = table.Column<string>(nullable: false),
                    MaxScore = table.Column<int>(nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    AssessmentType = table.Column<string>(nullable: true),
                    Instructions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.AssessmentId);
                    table.ForeignKey(
                        name: "FK_Assessments_LifeSkillCourses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "LifeSkillCourses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AssessmentResults",
                columns: table => new
                {
                    ResultId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentId = table.Column<int>(nullable: false),
                    StudentId = table.Column<int>(nullable: false),
                    Score = table.Column<int>(nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    Feedback = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentResults", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_Assessments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "LifeSkillCourses",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });
            // Notifications
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    StudentId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey("FK_Notifications_Students_StudentId", x => x.StudentId, "Students", "StudentId");
                });

            // CourseMaterials
            migrationBuilder.CreateTable(
                name: "CourseMaterials",
                columns: table => new
                {
                    MaterialId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    FilePath = table.Column<string>(nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMaterials", x => x.MaterialId);
                    table.ForeignKey("FK_CourseMaterials_LifeSkillCourses_CourseId", x => x.CourseId, "LifeSkillCourses", "CourseId", onDelete: ReferentialAction.Cascade);
                });

            // Feedbacks
            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    FeedbackDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedbackId);
                    table.ForeignKey("FK_Feedbacks_Students_StudentId", x => x.StudentId, "Students", "StudentId", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Feedbacks_LifeSkillCourses_CourseId", x => x.CourseId, "LifeSkillCourses", "CourseId", onDelete: ReferentialAction.Cascade);
                });

            // Certificates
            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CertificateCode = table.Column<string>(maxLength: 50, nullable: false),
                    FilePath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                    table.UniqueConstraint("UK_Certificates_CertificateCode", x => x.CertificateCode);
                    table.ForeignKey("FK_Certificates_Students_StudentId", x => x.StudentId, "Students", "StudentId", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Certificates_LifeSkillCourses_CourseId", x => x.CourseId, "LifeSkillCourses", "CourseId", onDelete: ReferentialAction.Cascade);
                });

            // CourseSchedules
            migrationBuilder.CreateTable(
                name: "CourseSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(nullable: false),
                    SessionDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    Room = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSchedules", x => x.ScheduleId);
                    table.ForeignKey("FK_CourseSchedules_LifeSkillCourses_CourseId", x => x.CourseId, "LifeSkillCourses", "CourseId", onDelete: ReferentialAction.Cascade);
                });

            // ActivityLogs
            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    Action = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.LogId);
                    table.ForeignKey("FK_ActivityLogs_Students_UserId", x => x.UserId, "Students", "StudentId");
                });

            // Payments
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Status = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey("FK_Payments_Students_StudentId", x => x.StudentId, "Students", "StudentId", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Payments_LifeSkillCourses_CourseId", x => x.CourseId, "LifeSkillCourses", "CourseId", onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    EnrollmentId = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    CompletionStatus = table.Column<bool>(nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.EnrollmentId);
                    table.ForeignKey("FK_Enrollments_Students_StudentId", x => x.StudentId, "Students", "StudentId", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Enrollments_LifeSkillCourses_CourseId", x => x.CourseId, "LifeSkillCourses", "CourseId", onDelete: ReferentialAction.Cascade);
                });
        }
    
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ActivityLogs");
            migrationBuilder.DropTable(name: "Assessments");
            migrationBuilder.DropTable(name: "AssessmentResults");
            migrationBuilder.DropTable(name: "Certificates");
            migrationBuilder.DropTable(name: "CourseMaterials");
            migrationBuilder.DropTable(name: "CourseSchedules");
            migrationBuilder.DropTable(name: "Feedbacks");
            migrationBuilder.DropTable(name: "Notifications");
            migrationBuilder.DropTable(name: "Payments");
            migrationBuilder.DropTable(name: "Students");
            migrationBuilder.DropTable(name: "LifeSkillCourses");
            migrationBuilder.DropTable(name: "Instructors");
            migrationBuilder.DropTable(name: "Enrollments");
        }
    }
}

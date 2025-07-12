using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Business_Layer
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<LifeSkillCourse> LifeSkillCourses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseSchedule> CourseSchedules { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentResult> AssessmentResults { get; set; }
        private static ApplicationDbContext _instance;
        private static readonly object _lock = new object();
        public static ApplicationDbContext Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                        IConfigurationRoot configuration = builder.Build();

                        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                        optionsBuilder.UseSqlServer(configuration.GetConnectionString("SkillDB"));

                        _instance = new ApplicationDbContext(optionsBuilder.Options);
                    }
                    return _instance;
                }
            }
        }

        private ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
            Console.WriteLine("Đang kết nối DB: " + this.Database.GetDbConnection().ConnectionString);

            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // định nghĩa các relationship nếu có
        }
    }
}

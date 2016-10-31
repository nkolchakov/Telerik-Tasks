using SchoolSystem.Data.Migrations;
using SchoolSystem.Models;
using System.Data.Entity;

namespace SchoolSystem.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext()
            : base("SchoolSystemCodeFirst")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SchoolDbContext,
                                    Configuration>());

        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Homework> Homeworks { get; set; }
        public virtual DbSet<Material> Materials { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                        .HasMany<Course>(s => s.Courses)
                        .WithMany(c => c.Students)
                        .Map(cs =>
                        {
                            cs.MapLeftKey("StudentRefID");
                            cs.MapRightKey("CourseRefID");
                            cs.ToTable("StudentsInCourses");
                        });

            modelBuilder.Entity<Course>()
                        .HasMany<Material>(c => c.Materials)
                        .WithMany(m => m.Courses)
                        .Map(cm =>
                        {
                            cm.MapLeftKey("CourseRefID");
                            cm.MapRightKey("MaterialRefID");
                            cm.ToTable("CourseWithMaterials");
                        });

            base.OnModelCreating(modelBuilder);
        }
    }
}

using Clinic.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Helper
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EmailConfirm> EmailConfirm { get; set; }
        public DbSet<DoctorViews> DoctorViews { get; set; }
        public DbSet<DoctorExperience> DoctorExperiences { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Pin> Pin { get; set; }

    }
}
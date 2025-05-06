using Assignment2;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    private readonly string _connectionString = "Server=DESKTOP-OS83RSF\\SQLEXPRESS;Database=OrmPractice;User Id=ormpractice;Password=123456;TrustServerCertificate=True";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Course> Course { get; set; }
    public DbSet<Instructor> Instructor { get; set; }
    public DbSet<Address> Address { get; set; }
    public DbSet<Phone> Phone { get; set; }
    public DbSet<Topic> Topic { get; set; }
    public DbSet<Session> Session { get; set; }
    public DbSet<AdmissionTest> AdmissionTest{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Instructor>()
           .HasOne<Course>()  // Each Instructor has one Course
           .WithMany()  // Each Course can have many Instructors
           .HasForeignKey(i => i.CourseId)  // Define the foreign key
           .OnDelete(DeleteBehavior.Restrict);

        // Define Address-Instructor Relationship (One-to-One)
        /*        modelBuilder.Entity<Address>()
                    .HasOne(a => a.Instructor)
                    .WithMany()
                    .HasForeignKey(a => a.InstructorId)
                    .OnDelete(DeleteBehavior.Restrict);*/

        // Define Phone-Instructor Relationship (One-to-Many)
        modelBuilder.Entity<Phone>()
            .HasOne<Instructor>()  // Define relationship without navigation
            .WithMany(i => i.PhoneNumbers)
            .HasForeignKey(p => p.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Define Topic-Course Relationship (Many-to-One)
        modelBuilder.Entity<Topic>()
            .HasOne<Course>()
            .WithMany(c => c.Topics)
            .HasForeignKey(t => t.CourseId);

        // Define Session-Topic Relationship (Many-to-One)
        modelBuilder.Entity<Session>()
            .HasOne<Topic>()
            .WithMany(t => t.Sessions)
            .HasForeignKey(s => s.TopicId);

        // Define AdmissionTest-Course Relationship (Many-to-One)
        modelBuilder.Entity<AdmissionTest>()
            .HasOne<Course>()
            .WithMany(c => c.Tests)
            .HasForeignKey(at => at.CourseId);

        // Define Instructor - Present Address (One-to-One)
        modelBuilder.Entity<Instructor>()
            .HasOne(i => i.PresentAddress)
            .WithMany()
            .HasForeignKey(i => i.PresentAddressId)
            .OnDelete(DeleteBehavior.Restrict);


        // Define Instructor - Permanent Address (One-to-One)
        modelBuilder.Entity<Instructor>()
            .HasOne(i => i.PermanentAddress)
            .WithMany()
            .HasForeignKey(i => i.PermanentAddressId)
            .OnDelete(DeleteBehavior.Restrict);


        base.OnModelCreating(modelBuilder);
    }
}

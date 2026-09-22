using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;

namespace TaskManagement.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
           DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }


        public DbSet<User> Users => Set<User>();

        public DbSet<Team> Teams => Set<Team>();

        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        public DbSet<Comment> Comments => Set<Comment>();

        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(x => x.Team)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.AssignedToUser)
                .WithMany(x => x.AssignedTasks)
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.Team)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.Task)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(x => x.Task)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

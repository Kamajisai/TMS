using Microsoft.EntityFrameworkCore;
using PtojectAPI.Entitys;
using PtojectAPI.FileUploads;
using PtojectAPI.Models;

namespace PtojectAPI.Datas
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Projectsetup> Projects { get; set; }
        public DbSet<Subtask> Tasks { get; set; }

        public DbSet<FileUpload> Files { get; set; }

        public DbSet<LinkUpload> Links { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; } // Add RefreshToken DbSet
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          


            // User Configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity < RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id); 

                entity.Property(rt => rt.Token)
                    .IsRequired()
                    .HasMaxLength (500);

                entity.Property(rt => rt.ExpiryDate)
                    .IsRequired();

                entity.Property(rt => rt.IsUsed)
                    .HasDefaultValue(false);

                entity.Property(rt => rt.IsRevoked)
                    .HasDefaultValue(false);

                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId);

            });

            // Tenant Configuration
            modelBuilder.Entity<Tenant>()
                .HasKey(t => t.TenantId);

            modelBuilder.Entity<Tenant>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tenants)
                .HasForeignKey(t => t.UserId) 
                .OnDelete(DeleteBehavior.Restrict); // 🔹 Fix: Change from Cascade to Restrict

            // Projectsetup Configuration
            modelBuilder.Entity<Projectsetup>()
                .HasKey(p => p.TitleId);

            modelBuilder.Entity<Projectsetup>()
                .Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Projectsetup>()
                .HasOne(p => p.Tenant)
                .WithMany(t => t.Projects)
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a Tenant deletes related Projects

            modelBuilder.Entity<Projectsetup>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 🔹 Prevents multiple cascade paths

            // Subtask Configuration
            modelBuilder.Entity<Subtask>()
                .HasKey(s => s.SubtaskId);              //add now 28/03

            modelBuilder.Entity<Subtask>()
                .Property(s => s.Task)
                .IsRequired()
                .HasMaxLength(300);

            modelBuilder.Entity<Subtask>()
                .HasOne(s => s.Projectsetup)
                .WithMany(p => p.Tasks)
                .HasForeignKey(s => s.TitleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subtask>()
                .HasOne(s => s.AssignedUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //file uploads configuration
            modelBuilder.Entity<FileUpload>()
                .HasKey(f => f.DocumentId);

            modelBuilder.Entity<FileUpload>()
                .HasOne(f => f.Subtask)
                .WithMany(s => s.fileUploads)
                .HasForeignKey(f => f.SubtaskId);

            modelBuilder.Entity<FileUpload>()
                .HasOne(f => f.UploadUser)
                .WithMany(u => u.UserFiles)
                .HasForeignKey(f => f.UserId);

            //Link uploads confihuration
            modelBuilder.Entity<LinkUpload>()
                .HasKey(l => l.LinkId);

            modelBuilder.Entity<LinkUpload>()
                .HasOne(l => l.UploadUser)
                .WithMany(u => u.UserLinks)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<LinkUpload>()
                .HasOne(l => l.Subtask)
                .WithMany(s => s.linkUploads)
                .HasForeignKey(l => l.SubtaskId);

            modelBuilder.Entity<Notification>()
                .HasKey(n => n.Id);

           modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    // User Configuration
        //    modelBuilder.Entity<User>()
        //        .HasKey(u => u.UserId);

        //    modelBuilder.Entity<User>()
        //        .HasIndex(u => u.Email)
        //        .IsUnique(); // Ensuring unique email

        //    modelBuilder.Entity<User>()
        //        .Property(u => u.Name)
        //        .IsRequired()
        //        .HasMaxLength(100);

        //    // Tenant Configuration
        //    modelBuilder.Entity<Tenant>()
        //        .HasKey(t => t.TenantId);

        //    modelBuilder.Entity<Tenant>()
        //        .HasOne(t => t.User)
        //        .WithMany()
        //        .HasForeignKey(t => t.UserId)
        //        .OnDelete(DeleteBehavior.Cascade); // Deleting a User deletes related Tenants

        //    // Projectsetup Configuration
        //    modelBuilder.Entity<Projectsetup>()
        //        .HasKey(p => p.TitleId);

        //    modelBuilder.Entity<Projectsetup>()
        //        .Property(p => p.Title)
        //        .IsRequired()
        //        .HasMaxLength(200);

        //    modelBuilder.Entity<Projectsetup>()
        //        .HasOne(p => p.Tenant)
        //        .WithMany()
        //        .HasForeignKey(p => p.TenantId)
        //        .OnDelete(DeleteBehavior.Cascade); // Deleting a Tenant deletes related Projects

        //    modelBuilder.Entity<Projectsetup>()
        //        .HasOne(p => p.User) // Fix: Prevent multiple cascade paths
        //        .WithMany()
        //        .HasForeignKey(p => p.UserId)
        //        .OnDelete(DeleteBehavior.Restrict); // Prevents deletion issue

        //    // Subtask Configuration
        //    modelBuilder.Entity<Subtask>()
        //        .Property(s => s.Task)
        //        .IsRequired()
        //        .HasMaxLength(300);

        //    modelBuilder.Entity<Subtask>()
        //        .HasOne(s => s.Projectsetup)
        //        .WithMany(p => p.Tasks)
        //        .HasForeignKey(s => s.TitleId)
        //        .OnDelete(DeleteBehavior.Cascade); // Deleting a Project deletes related Subtasks

        //    modelBuilder.Entity<Subtask>()
        //        .HasOne(s => s.AssignedUser)
        //        .WithMany(u => u.AssignedTasks)
        //        .HasForeignKey(s => s.UserId)
        //        .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of assigned users

        //}
    }
}

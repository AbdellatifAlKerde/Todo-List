using Microsoft.EntityFrameworkCore;

namespace Todo_List.Models
{
    public class TodoDBContext : DbContext
    {
        public TodoDBContext(DbContextOptions<TodoDBContext> options) : base(options) { }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .HasOne<User>()            
                .WithMany(u => u.Tasks)         
                .HasForeignKey(t => t.UserId); 

            modelBuilder.Entity<Task>()
                .Property(p => p.Status)
                .HasDefaultValue("todo");  

            base.OnModelCreating(modelBuilder);
        }

        
    }
}

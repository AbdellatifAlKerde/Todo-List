namespace Todo_List.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public string? PasswordHash { get; set; }

        public ICollection<Task>? Tasks { get; set; } = new List<Task>();
    }
}

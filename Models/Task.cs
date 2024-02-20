namespace Todo_List.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Estimate { get; set; }
        public string? Importance { get; set; }
        public string? Status { get; set; }

        public int UserId { get; set; }
        // public User? User { get; set; }
    }
}

namespace TaskManagement.API.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "To Do";

        public string Priority { get; set; } = "Medium";

        public DateTime Deadline { get; set; }

        public int AssignedToUserId { get; set; }

        public User AssignedToUser { get; set; } = null!;

        public int? TeamId { get; set; }

        public Team? Team { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}

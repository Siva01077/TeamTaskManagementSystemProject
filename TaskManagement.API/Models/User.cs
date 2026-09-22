namespace TaskManagement.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public int? TeamId { get; set; }

        public Team? Team { get; set; }

        public ICollection<TaskItem> AssignedTasks { get; set; }
            = new List<TaskItem>();

        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}

namespace TaskManagement.API.Models
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; }
            = new List<User>();

        public ICollection<TaskItem> Tasks { get; set; }
            = new List<TaskItem>();
    }
}

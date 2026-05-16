using System.ComponentModel.DataAnnotations;

namespace TrainingPlatform.MVC.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
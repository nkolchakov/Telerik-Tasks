using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Models
{
    public class Material
    {
        private ICollection<Course> courses;

        public Material()
        {
            this.courses = new HashSet<Course>();
        }

        public int ID { get; set; }

        [Required]
        [MaxLength(30)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(50)]
        public string Lector { get; set; }

        public int? Credits { get; set; }

        public ICollection<Course> Courses
        {
            get { return this.courses; }
            set { this.courses = value; }
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Models
{
    public class Homework
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(150)]
        public string Content { get; set; }

        public DateTime? TimeSent { get; set; }

        public int StudentID { get; set; }

        public Student Student { get; set; }

        public int CourseID { get; set; }

        public Course Course { get; set; }


    }
}

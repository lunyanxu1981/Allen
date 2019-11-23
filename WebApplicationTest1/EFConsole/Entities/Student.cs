using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFConsole.Entities
{
    public class Student
    {
        [Required]
        public int StudentID { get; set; }

        [StringLength(200)]
        public string StudentName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public byte[] Photo { get; set; }
        public decimal? Height { get; set; }
        public float? Weight { get; set; }

        public Grade Grade { get; set; }
    }
}

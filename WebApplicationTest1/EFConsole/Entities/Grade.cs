using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EFConsole.Entities
{
    public class Grade
    {
        [Required]
        public int GradeId { get; set; }
        [StringLength(50)]
        public string GradeName { get; set; }
        [StringLength(50)]
        public string Section { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}

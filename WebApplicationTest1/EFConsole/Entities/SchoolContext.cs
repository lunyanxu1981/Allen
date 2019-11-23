using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EFConsole.Entities
{
    public class SchoolContext : DbContext
    {
        public SchoolContext() : base("name=SchoolContext")
        { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }
    }

   
}

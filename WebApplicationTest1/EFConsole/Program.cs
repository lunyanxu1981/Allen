using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using EFConsole.Entities;

namespace EFConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteEFCodeFirst();
        }

        public static void ExecuteEFCodeFirst()
        {
            using (var context = new SchoolContext())
            {
                var stu = new Student()
                {
                    StudentID = 2,
                    StudentName = "Kobe"
                };

                context.Students.Add(stu);
                context.SaveChanges();
            }
        }
    }
}

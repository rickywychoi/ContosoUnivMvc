using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.ViewModels
{
    public class StudentsListVM
    {
        public string NameSortParam { get; set; }
        public string DateSortParam { get; set; }
        public IEnumerable<Student> StudentsList { get; set; }
    }
}

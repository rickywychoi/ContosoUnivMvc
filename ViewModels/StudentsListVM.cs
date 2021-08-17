using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.ViewModels
{
    public class StudentsListVM
    {
        public string NameSortParam { get; set; } // "name_asc" or "name_desc"
        public string DateSortParam { get; set; } // "date_asc" or "date_desc"
        public string SearchString { get; set; } = "";
        public string CurrentSort { get; set; } // place to store the current sorting option
        public PaginatedList<Student> StudentsList { get; set; }
    }
}

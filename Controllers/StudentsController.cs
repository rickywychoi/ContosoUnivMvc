using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;
        
        // Constants for sorting order in index page.
        private const string NAME_DESC = "name_desc";
        private const string DATE_DESC = "date_desc";
        private const string DATE_ASC = "date_asc";

        public StudentsListVM StudentsListViewModel { get; set; } = new StudentsListVM
        {
            NameSortParam = NAME_DESC,
            DateSortParam = DATE_DESC
        };

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, 
            string searchString, 
            string currentFilter,
            int? pageNumber)
        {
            StudentsListViewModel.CurrentSort = sortOrder;
            StudentsListViewModel.NameSortParam = String.IsNullOrEmpty(sortOrder) ? NAME_DESC : "";
            StudentsListViewModel.DateSortParam = sortOrder == DATE_ASC ? DATE_DESC : DATE_ASC;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            StudentsListViewModel.SearchString = searchString;
            
            var students = _context.Students.AsQueryable();

            // filter students by search string first.
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString) 
                    || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case NAME_DESC:
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case DATE_ASC:
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case DATE_DESC:
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;

            StudentsListViewModel.StudentsList = await PaginatedList<Student>.CreateAsync(students.AsNoTracking(),
                    pageNumber ?? 1,
                    pageSize
                );

            return View(StudentsListViewModel);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] // prevent CSRF attacks
        public async Task<IActionResult> Create([Bind("LastName,FirstMidName,EnrollmentDate")] Student student) // No need to bind Id cuz db auto generates it
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                // Log the error
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            // TODO: Comment out the below and uncomment the original logic
            return await AlternativeEditAsync(id, student);

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            //if (await TryUpdateModelAsync<Student>(
            //        studentToUpdate,
            //        "",
            //        s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate
            //    ))
            //{
            //    try
            //    {
            //        await _context.SaveChangesAsync();
            //        return RedirectToAction(nameof(Index));
            //    }
            //    catch (DbUpdateException)
            //    {
            //        ModelState.AddModelError("", "Unable to save changes. " +
            //            "Try again, and if the problem persists, " +
            //            "see your system administrator.");
            //    }
            //}
            //return View(studentToUpdate);
        }

        /// <summary>
        /// Alternative approach to edit and save the entity state.
        /// </summary>
        /// <param name="id">Id of the student to edit</param>
        /// <param name="student">Student model to edit</param>
        /// <returns></returns>
        private async Task<IActionResult> AlternativeEditAsync(int id, [Bind("Id,LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // TODO: Comment out the below and uncomment the original logic
            return await FasterDeleteAsync(id);

            //var student = await _context.Students.FindAsync(id);
            //if (student is null)
            //{
            //    return RedirectToAction(nameof(Index));
            //}

            //try
            //{
            //    _context.Students.Remove(student);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //catch (DbUpdateException)
            //{
            //    return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            //}
        }

        private async Task<IActionResult> FasterDeleteAsync(int id)
        {
            try
            {
                var studentToDelete = new Student { Id = id }; // create temp student entity with Id to delete
                _context.Entry(studentToDelete).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}

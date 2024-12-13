using Cumulative_Project1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cumulative_Project1.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly TeacherAPIController _api;

        // Dependency injection of the TeacherAPIController
        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }

        /// <summary>
        /// Lists all teachers. Supports optional search functionality.
        /// </summary>
        /// <example>
        /// GET: TeacherPage/List?SearchKey=John
        /// </example>
        /// <param name="SearchKey">Optional search key for filtering teachers by name.</param>
        /// <returns>
        /// A view displaying the list of teachers.
        /// </returns>
        [HttpGet]
        public IActionResult List(string SearchKey = null)
        {
            List<Teacher> Teachers = _api.ListTeachers(SearchKey);
            return View(Teachers);
        }

        /// <summary>
        /// Displays the details of a single teacher.
        /// </summary>
        /// <example>
        /// GET: TeacherPage/Show/1
        /// </example>
        /// <param name="id">The ID of the teacher to display.</param>
        /// <returns>
        /// A view displaying the teacher's details.
        /// </returns>
        [HttpGet]
        public IActionResult Show(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        /// <summary>
        /// Displays a form to create a new teacher.
        /// </summary>
        /// <example>
        /// GET: TeacherPage/New
        /// </example>
        /// <returns>
        /// A view with the teacher creation form.
        /// </returns>
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Creates a new teacher and redirects to the Show action.
        /// </summary>
        /// <example>
        /// POST: TeacherPage/Create
        /// </example>
        /// <param name="NewTeacher">The teacher data to create.</param>
        /// <returns>
        /// A redirect to the Show action for the created teacher.
        /// </returns>
        [HttpPost]
        public IActionResult Create(Teacher NewTeacher)
        {
            // Call AddTeacher and get the result
            IActionResult result = _api.AddTeacher(NewTeacher);

            // If there is a validation error (BadRequest)
            if (result is BadRequestObjectResult)
            {
                var error = ((BadRequestObjectResult)result).Value;
                // Handle error and return the same view with the error message
                ModelState.AddModelError("", error.ToString());
                return View("New", NewTeacher); // Return to the creation form with error message
            }

            // If the result is a success (OkObjectResult), get the TeacherId
            var teacherId = ((OkObjectResult)result).Value;
            return RedirectToAction("Show", new { id = teacherId });
        }

        /// <summary>
        /// Displays a confirmation page for deleting a teacher.
        /// </summary>
        /// <example>
        /// GET: TeacherPage/DeleteConfirm/1
        /// </example>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>
        /// A view confirming the teacher to delete.
        /// </returns>
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        /// <summary>
        /// Deletes a teacher and redirects to the List action.
        /// </summary>
        /// <example>
        /// POST: TeacherPage/Delete/1
        /// </example>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>
        /// A redirect to the List action.
        /// </returns>
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _api.DeleteTeacher(id);
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays a form to edit an existing teacher.
        /// </summary>
        /// <example>
        /// GET: TeacherPage/Edit/1
        /// </example>
        /// <param name="id">The ID of the teacher to edit.</param>
        /// <returns>
        /// A view with the teacher edit form.
        /// </returns>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        /// <summary>
        /// Updates an existing teacher and redirects to the Show action.
        /// </summary>
        /// <example>
        /// POST: TeacherPage/Update/1
        /// </example>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="updatedTeacher">The teacher object with updated details.</param>
        /// <returns>
        /// A redirect to the Show action for the updated teacher.
        /// </returns>
        [HttpPost]
        public IActionResult Update(int id, Teacher updatedTeacher)
        {
            // Ensure updatedTeacher contains the ID
            updatedTeacher.TeacherId = id;

            // Call the API to update the teacher
            var result = _api.UpdateTeacher(id, updatedTeacher);

            // Redirect to the show page if the update is successful
            return RedirectToAction("Show", new { id = id });
        }
    }
}

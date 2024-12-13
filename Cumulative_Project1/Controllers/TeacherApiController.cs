using Cumulative_Project1.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cumulative_Project1.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        // Dependency injection of database context
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        // Method to check if EmployeeNumber is unique
        private bool IsEmployeeNumberUnique(string employeeNumber)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "SELECT COUNT(*) FROM teachers WHERE employeenumber = @employeenumber";
                Command.Parameters.AddWithValue("@employeenumber", employeeNumber);

                int count = Convert.ToInt32(Command.ExecuteScalar());
                return count == 0;  // If count is 0, it's unique
            }
        }

        // Method to validate the salary range
        private bool IsSalaryValid(decimal salary)
        {
            return salary >= 30000;  // Example: Salary must be at least 30,000
        }

        // Method to validate employee number format using regex (T followed by 4 digits)
        private bool IsEmployeeNumberFormatValid(string employeeNumber)
        {
            // Ensure the employee number starts with 'T' and is followed by 4 digits
            var regex = new Regex(@"^T\d{4}$");
            return regex.IsMatch(employeeNumber);
        }

        /// <summary>
        /// Returns a list of teachers in the system. Optional search key filters teachers by first or last name.
        /// </summary>
        [HttpGet]
        [Route("ListTeachers")]
        public List<Teacher> ListTeachers(string SearchKey = null)
        {
            List<Teacher> Teachers = new List<Teacher>();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                string query = "SELECT * FROM teachers";
                if (SearchKey != null)
                {
                    query += " WHERE LOWER(teacherfname) LIKE @key OR LOWER(teacherlname) LIKE @key OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE @key";
                    Command.Parameters.AddWithValue("@key", $"%{SearchKey.ToLower()}%");
                }
                Command.CommandText = query;
                Command.Prepare();

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        Teacher CurrentTeacher = new Teacher()
                        {
                            TeacherId = Convert.ToInt32(ResultSet["teacherid"]),
                            TeacherFirstName = ResultSet["teacherfname"].ToString(),
                            TeacherLastName = ResultSet["teacherlname"].ToString(),
                            EmployeeNumber = ResultSet["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                            Salary = Convert.ToDecimal(ResultSet["salary"])
                        };

                        Teachers.Add(CurrentTeacher);
                    }
                }
            }

            return Teachers;
        }

        /// <summary>
        /// Returns a teacher in the database by their ID
        /// </summary>
        [HttpGet]
        [Route("FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher SelectedTeacher = new Teacher();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        SelectedTeacher = new Teacher()
                        {
                            TeacherId = Convert.ToInt32(ResultSet["teacherid"]),
                            TeacherFirstName = ResultSet["teacherfname"].ToString(),
                            TeacherLastName = ResultSet["teacherlname"].ToString(),
                            EmployeeNumber = ResultSet["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                            Salary = Convert.ToDecimal(ResultSet["salary"])
                        };
                    }
                }
            }

            return SelectedTeacher;
        }

        /// <summary>
        /// Adds a new teacher to the database
        /// </summary>
        [HttpPost]
        [Route("AddTeacher")]
        public IActionResult AddTeacher([FromBody] Teacher teacherData)
        {
            if (!IsEmployeeNumberFormatValid(teacherData.EmployeeNumber))
            {
                return BadRequest("Employee number must be in the correct format (e.g., T0001).");
            }

            if (!IsEmployeeNumberUnique(teacherData.EmployeeNumber))
            {
                return BadRequest("Employee number must be unique.");
            }

            if (!IsSalaryValid(teacherData.Salary))
            {
                return BadRequest("Salary must be at least 30,000.");
            }

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) " +
                                      "VALUES (@teacherfname, @teacherlname, @employeenumber, @hiredate, @salary)";
                Command.Parameters.AddWithValue("@teacherfname", teacherData.TeacherFirstName);
                Command.Parameters.AddWithValue("@teacherlname", teacherData.TeacherLastName);
                Command.Parameters.AddWithValue("@employeenumber", teacherData.EmployeeNumber);
                Command.Parameters.AddWithValue("@hiredate", teacherData.HireDate);
                Command.Parameters.AddWithValue("@salary", teacherData.Salary);

                Command.ExecuteNonQuery();
            }

            return Ok(new { TeacherId = teacherData.TeacherId });
        }

        /// <summary>
        /// Updates an existing teacher in the database
        /// </summary>
        [HttpPut]
        [Route("UpdateTeacher/{id}")]
        public IActionResult UpdateTeacher(int id, [FromBody] Teacher teacherData)
        {
            // Validate employee number format
            if (!IsEmployeeNumberFormatValid(teacherData.EmployeeNumber))
            {
                return BadRequest("Employee number must be in the correct format (e.g., T0001).");
            }

            // Validate employee number uniqueness (skip check if it's the same as before)
            Teacher existingTeacher = FindTeacher(id);
            if (existingTeacher != null && existingTeacher.EmployeeNumber != teacherData.EmployeeNumber && !IsEmployeeNumberUnique(teacherData.EmployeeNumber))
            {
                return BadRequest("Employee number must be unique.");
            }

            // Validate salary
            if (!IsSalaryValid(teacherData.Salary))
            {
                return BadRequest("Salary must be at least 30,000.");
            }

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "UPDATE teachers SET teacherfname = @teacherfname, teacherlname = @teacherlname, " +
                                      "employeenumber = @employeenumber, hiredate = @hiredate, salary = @salary " +
                                      "WHERE teacherid = @id";
                Command.Parameters.AddWithValue("@teacherfname", teacherData.TeacherFirstName);
                Command.Parameters.AddWithValue("@teacherlname", teacherData.TeacherLastName);
                Command.Parameters.AddWithValue("@employeenumber", teacherData.EmployeeNumber);
                Command.Parameters.AddWithValue("@hiredate", teacherData.HireDate);
                Command.Parameters.AddWithValue("@salary", teacherData.Salary);
                Command.Parameters.AddWithValue("@id", id);

                Command.ExecuteNonQuery();
            }

            return Ok(FindTeacher(id));  // After updating, return the updated teacher
        }

        /// <summary>
        /// Deletes a teacher from the database
        /// </summary>
        [HttpDelete]
        [Route("DeleteTeacher/{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
                Command.Parameters.AddWithValue("@id", id);

                Command.ExecuteNonQuery();
            }

            return NoContent();  // Successful deletion
        }
    }
}

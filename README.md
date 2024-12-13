# Teacher Management System

This application allows users to manage teacher data by adding, deleting, and viewing teacher records. It uses ASP.NET Core Web API for backend functionality and MVC for front-end views, with a MySQL database to store teacher information.

## Features

- **Add Teacher**: Add new teacher details such as name, employee number, hire date, and phone number.
- **Delete Teacher**: Confirm and delete teacher records.
- **View Teachers**: List all teachers in the database.

## Installation

### Step 1: Install MySQL
1. Set up a MySQL environment (e.g., MAMP or XAMPP).
2. Import the `teachers.sql` file to create the `school` database.

### Step 2: Update Database Connection
1. Modify the connection string in `SchoolDbContext.cs` to match your MySQL settings.

### Step 3: Run the Application
1. Ensure your MySQL server is running.
2. Press **F5** or **Ctrl + F5** to run the project in Visual Studio.

## API Endpoints

- **POST /api/Teacher/AddTeacher**: Add a new teacher.
- **DELETE /api/Teacher/DeleteTeacher/{id}**: Delete a teacher by ID.
- **GET /api/Teacher/ListTeachers**: Get a list of all teachers.

## MVC Views

- **/Teacher/New**: Page to add a new teacher.
- **/Teacher/DeleteConfirm/{id}**: Page to confirm teacher deletion.

## How to Use

1. Go to the **Add Teacher** page to add a teacher.
2. Go to the **Teacher List** to view all teachers and delete them if needed.
3. Confirm deletion on the confirmation page.

---

For more detailed instructions, please refer to the full documentation.

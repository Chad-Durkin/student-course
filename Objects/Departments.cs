using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar
{
    public class Department
    {
        private string _name;
        private int _id;

        public Department(string name, int id = 0)
        {
            _name = name;
            _id = id;
        }

        public override bool Equals(System.Object otherDepartment)
        {
            if(!(otherDepartment is Department))
            {
                return false;
            }
            else
            {
                Department newDepartment = (Department) otherDepartment;
                bool idEquality = this.GetId() == newDepartment.GetId();
                bool nameEquality = this.GetName() == newDepartment.GetName();
                return (idEquality && nameEquality);
            }
        }

        //HashCode is a unique identifier given by computer relating to its location in the computer's memory. the following method overrides the unique id and sets it equal to name
        public override int GetHashCode()
        {
            return this.GetName().GetHashCode();
        }

        public int GetId()
        {
            return _id;
        }
        public void SetId(int id)
        {
            _id = id;
        }
        public string GetName()
        {
            return _name;
        }
        public void SetName(string name)
        {
            _name = name;
        }

        public static List<Department> GetAll()
        {
            List<Department> allDepartments = new List<Department>{};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM department", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                int departmentId = rdr.GetInt32(0);
                string departmentName = rdr.GetString(1);
                Department newDepartment = new Department(departmentName, departmentId);
                allDepartments.Add(newDepartment);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allDepartments;
        }

        public void AddStudent(int studentId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE students SET department_id = @DepartmentId WHERE id = @StudentId;", conn);

            cmd.Parameters.Add(new SqlParameter("@StudentId", studentId));
            cmd.Parameters.Add(new SqlParameter("@DepartmentId", this.GetId().ToString()));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }

        public void AddCourse(int courseId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE courses SET department_id = @DepartmentId WHERE id = @CourseId;", conn);

            cmd.Parameters.Add(new SqlParameter("@CourseId", courseId));
            cmd.Parameters.Add(new SqlParameter("@DepartmentId", this.GetId().ToString()));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }

        public List<Student> GetStudents()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE department_id = @DepartmentId;", conn);
            cmd.Parameters.Add(new SqlParameter("@DepartmentId", this.GetId().ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<Student> foundStudents = new List<Student>{};

            while(rdr.Read())
            {
                int foundId = rdr.GetInt32(0);
                string studentName = rdr.GetString(1);
                string enrollmentDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd");
                int departmentId = rdr.GetInt32(3);
                Student newStudent = new Student(studentName, enrollmentDate, departmentId, foundId);
                foundStudents.Add(newStudent);
            }

            DB.CloseSqlConnection(rdr, conn);

            return foundStudents;
        }

        public List<Course> GetCourses()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM courses WHERE department_id = @DepartmentId;", conn);
            cmd.Parameters.Add(new SqlParameter("@DepartmentId", this.GetId().ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<Course> foundCourses = new List<Course>{};

            while(rdr.Read())
            {
                int foundId = rdr.GetInt32(0);
                string courseName = rdr.GetString(1);
                string courseNumber = rdr.GetString(2);
                int departmentId = rdr.GetInt32(3);
                Course newCourse = new Course(courseName, courseNumber, departmentId, foundId);
                foundCourses.Add(newCourse);
            }

            DB.CloseSqlConnection(rdr, conn);

            return foundCourses;
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO department (name) OUTPUT INSERTED.id VALUES (@Name);", conn);

            cmd.Parameters.Add(new SqlParameter("@Name", this.GetName()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public void Delete()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM department WHERE id = @DepartmentId; DELETE FROM courses WHERE department_id = @DepartmentId; DELETE FROM students WHERE department_id = @DepartmentId;", conn);
            cmd.Parameters.Add(new SqlParameter("@DepartmentId", this.GetId()));

            cmd.ExecuteNonQuery();

            if (conn != null)
            {
                conn.Close();
            }
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("department");
        }
    }
}

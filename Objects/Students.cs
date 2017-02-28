using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar
{
    public class Student
    {
        private string _name;
        private string _enrollmentDate;
        private int _id;
        private int _departmentId;

        public Student(string name, string enrollmentDate, int departmentId = 0, int id = 0)
        {
            _name = name;
            _enrollmentDate = enrollmentDate;
            _id = id;
            _departmentId = departmentId;
        }

        public override bool Equals(System.Object otherStudent)
        {
            if(!(otherStudent is Student))
            {
                return false;
            }
            else
            {
                Student newStudent = (Student) otherStudent;
                bool idEquality = this.GetId() == newStudent.GetId();
                bool nameEquality = this.GetName() == newStudent.GetName();
                bool enrollmentDateEquality = this.GetEnrollmentDate() == newStudent.GetEnrollmentDate();
                bool departmentIdEqualtiy = this.GetDepartmentId() == newStudent.GetDepartmentId();
                return (idEquality && nameEquality && enrollmentDateEquality);
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
        public int GetDepartmentId()
        {
            return _departmentId;
        }
        public void SetDepartmentId(int departmentId)
        {
            _departmentId = departmentId;
        }
        public string GetName()
        {
            return _name;
        }
        public void SetName(string name)
        {
            _name = name;
        }
        public string GetEnrollmentDate()
        {
            return _enrollmentDate;
        }
        public void SetEnrollmentDate(string enrollmentDate)
        {
            _enrollmentDate = enrollmentDate;
        }

        public static List<Student> GetAll()
        {
            List<Student> allStudents = new List<Student>{};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM students", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                int studentId = rdr.GetInt32(0);
                string studentName = rdr.GetString(1);
                string studentEnrollmentDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd");
                int departmentId = rdr.GetInt32(3);
                Student newStudent = new Student(studentName, studentEnrollmentDate, departmentId, studentId);
                allStudents.Add(newStudent);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allStudents;
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment_date, department_id) OUTPUT INSERTED.id VALUES (@Name, @EnrollmentDate, @DepartmentId);", conn);

            cmd.Parameters.Add(new SqlParameter("@Name", this.GetName()));
            cmd.Parameters.Add(new SqlParameter("@EnrollmentDate", this.GetEnrollmentDate()));
            cmd.Parameters.Add(new SqlParameter("@DepartmentId", this.GetDepartmentId()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public void Add(int courseId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (courses_id, students_id, completed) VALUES (@CourseId, @StudentId, @Completed);", conn);

            cmd.Parameters.Add(new SqlParameter("@CourseId", courseId.ToString()));
            cmd.Parameters.Add(new SqlParameter("@StudentId", this.GetId().ToString()));
            cmd.Parameters.Add(new SqlParameter("@Completed", "0"));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }

        public List<Course> GetCourses()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT courses.* FROM students JOIN courses_students ON (students.id = courses_students.students_id) JOIN courses ON (courses_students.courses_id = courses.id) WHERE students.id = @StudentId;", conn);

            cmd.Parameters.Add(new SqlParameter("@StudentId", this.GetId().ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<Course> allCourses = new List<Course> {};

            while(rdr.Read())
            {
                int courseId = rdr.GetInt32(0);
                string courseName = rdr.GetString(1);
                string courseNumber = rdr.GetString(2);
                int departmentId = rdr.GetInt32(3);
                Course newCourse = new Course(courseName, courseNumber, departmentId, courseId);
                allCourses.Add(newCourse);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allCourses;
        }

        public static Student Find(int studentId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE id = @StudentId;", conn);
            cmd.Parameters.Add(new SqlParameter("@StudentId", studentId));

            SqlDataReader rdr = cmd.ExecuteReader();

            int foundId = 0;
            string studentName = null;
            string enrollmentDate = null;
            int departmentId = 0;

            while(rdr.Read())
            {
                foundId = rdr.GetInt32(0);
                studentName = rdr.GetString(1);
                enrollmentDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd");
                departmentId = rdr.GetInt32(3);
            }

            Student foundStudent = new Student(studentName, enrollmentDate, departmentId, foundId);

            DB.CloseSqlConnection(rdr, conn);

            return foundStudent;
        }

        public void Delete()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM students WHERE id = @StudentId; DELETE FROM courses_students WHERE students_id = @StudentId;", conn);
            cmd.Parameters.Add(new SqlParameter("@StudentId", this.GetId()));

            cmd.ExecuteNonQuery();

            if (conn != null)
            {
                conn.Close();
            }
        }

        public int GetCompleted(int courseId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT completed FROM courses_students WHERE courses_id = @CoursesId AND students_id = @StudentsId;", conn);

            cmd.Parameters.Add(new SqlParameter("@StudentsId", this.GetId()));
            cmd.Parameters.Add(new SqlParameter("@CoursesId", courseId.ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            int completedCourse = 0;

            while(rdr.Read())
            {
                completedCourse = rdr.GetByte(0);
            }

            DB.CloseSqlConnection(rdr, conn);

            return completedCourse;
        }

        public void UpdateCompleted(int courseId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE courses_students SET completed = @Completed WHERE courses_id = @CourseId AND students_id = @StudentId", conn);

            cmd.Parameters.Add(new SqlParameter("@StudentId", this.GetId()));
            cmd.Parameters.Add(new SqlParameter("@CourseId", courseId.ToString()));
            cmd.Parameters.Add(new SqlParameter("@Completed", "1"));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("students");
        }
    }
}

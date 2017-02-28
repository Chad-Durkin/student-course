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

        public Student(string name, string enrollmentDate, int id = 0)
        {
            _name = name;
            _enrollmentDate = enrollmentDate;
            _id = id;
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
                Student newStudent = new Student(studentName, studentEnrollmentDate, studentId);
                allStudents.Add(newStudent);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allStudents;
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment_date) OUTPUT INSERTED.id VALUES (@Name, @EnrollmentDate);", conn);

            cmd.Parameters.Add(new SqlParameter("@Name", this.GetName()));
            cmd.Parameters.Add(new SqlParameter("@EnrollmentDate", this.GetEnrollmentDate()));

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

            SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (courses_id, students_id) VALUES (@CourseId, @StudentId);", conn);

            cmd.Parameters.Add(new SqlParameter("@CourseId", courseId.ToString()));
            cmd.Parameters.Add(new SqlParameter("@StudentId", this.GetId().ToString()));

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
                Course newCourse = new Course(courseName, courseNumber, courseId);
                allCourses.Add(newCourse);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allCourses;
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("students");
        }
    }
}

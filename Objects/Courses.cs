using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar
{
    public class Course
    {
        private string _name;
        private string _courseNumber;
        private int _id;

        public Course(string name, string courseNumber, int id = 0)
        {
            _name = name;
            _courseNumber = courseNumber;
            _id = id;
        }

        public override bool Equals(System.Object otherCourse)
        {
            if(!(otherCourse is Course))
            {
                return false;
            }
            else
            {
                Course newCourse = (Course) otherCourse;
                bool idEquality = this.GetId() == newCourse.GetId();
                bool nameEquality = this.GetName() == newCourse.GetName();
                bool courseNumberEquality = this.GetCourseNumber() == newCourse.GetCourseNumber();
                return (idEquality && nameEquality && courseNumberEquality);
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
        public string GetCourseNumber()
        {
            return _courseNumber;
        }
        public void SetCourseNumber(string courseNumber)
        {
            _courseNumber = courseNumber;
        }

        public static List<Course> GetAll()
        {
            List<Course> allCourses = new List<Course>{};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM courses", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

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


        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO courses (name, course_number) OUTPUT INSERTED.id VALUES (@Name, @CourseNumber);", conn);

            cmd.Parameters.Add(new SqlParameter("@Name", this.GetName()));
            cmd.Parameters.Add(new SqlParameter("@CourseNumber", this.GetCourseNumber()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public void Add(int studentId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (courses_id, students_id) VALUES (@CourseId, @StudentId);", conn);

            cmd.Parameters.Add(new SqlParameter("@CourseId", this.GetId()));
            cmd.Parameters.Add(new SqlParameter("@StudentId", studentId.ToString()));

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

            SqlCommand cmd = new SqlCommand("SELECT students.* FROM courses JOIN courses_students ON (courses.id = courses_students.courses_id) JOIN students ON (courses_students.students_id = students.id) WHERE courses.id = @CourseId;", conn);

            cmd.Parameters.Add(new SqlParameter("@CourseId", this.GetId()));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<Student> allStudents = new List<Student> {};

            while(rdr.Read())
            {
                int studentId = rdr.GetInt32(0);
                string studentName = rdr.GetString(1);
                string enrollmentDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd");
                Student newStudent = new Student(studentName, enrollmentDate, studentId);
                allStudents.Add(newStudent);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allStudents;
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("courses");
        }
    }
}

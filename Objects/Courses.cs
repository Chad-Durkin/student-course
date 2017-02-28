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

            SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (courses_id, students_id, completed) VALUES (@CourseId, @StudentId, @Completed);", conn);

            cmd.Parameters.Add(new SqlParameter("@CourseId", this.GetId()));
            cmd.Parameters.Add(new SqlParameter("@StudentId", studentId.ToString()));
            cmd.Parameters.Add(new SqlParameter("@Completed", "0"));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }

        public int GetCompleted(int studentId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT completed FROM courses_students WHERE courses_id = @CoursesId AND students_id = @StudentsId;", conn);

            cmd.Parameters.Add(new SqlParameter("@CoursesId", this.GetId()));
            cmd.Parameters.Add(new SqlParameter("@StudentsId", studentId.ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            int completedCourse = 0;

            while(rdr.Read())
            {
                completedCourse = rdr.GetByte(0);
            }

            DB.CloseSqlConnection(rdr, conn);

            return completedCourse;
        }

        public void UpdateCompleted(int studentId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE courses_students SET completed = @Completed WHERE courses_id = @CourseId AND students_id = @StudentId", conn);

            cmd.Parameters.Add(new SqlParameter("@CourseId", this.GetId()));
            cmd.Parameters.Add(new SqlParameter("@StudentId", studentId.ToString()));
            cmd.Parameters.Add(new SqlParameter("@Completed", "1"));

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

            cmd.Parameters.Add(new SqlParameter("@CourseId", this.GetId().ToString()));

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

        public static Course Find(int courseId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM courses WHERE id = @CourseId;", conn);
            cmd.Parameters.Add(new SqlParameter("@CourseId", courseId));

            SqlDataReader rdr = cmd.ExecuteReader();

            int foundCourseId = 0;
            string courseName = null;
            string courseNumber = null;

            while(rdr.Read())
            {
                foundCourseId = rdr.GetInt32(0);
                courseName = rdr.GetString(1);
                courseNumber = rdr.GetString(2);
            }

            Course foundCourse = new Course(courseName, courseNumber, foundCourseId);

            DB.CloseSqlConnection(rdr, conn);

            return foundCourse;
        }

        public void Delete()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM courses WHERE id = @CourseId; DELETE FROM courses_students WHERE courses_id = @CourseId;", conn);
            cmd.Parameters.Add(new SqlParameter("@CourseId", this.GetId()));

            cmd.ExecuteNonQuery();

            if (conn != null)
            {
                conn.Close();
            }
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("courses");
        }
    }
}

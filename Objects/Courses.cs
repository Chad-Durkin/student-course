using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar
{
    public class Course
    {
        private string _name;
        private string _courseNumber;
        private int _departmentId;
        private int _id;

        public Course(string name, string courseNumber, int departmentId = 0, int id = 0)
        {
            _name = name;
            _courseNumber = courseNumber;
            _departmentId = departmentId;
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
                bool departmentIdEquality = this.GetDepartmentId() == newCourse.GetDepartmentId();
                return (idEquality && nameEquality && departmentIdEquality && courseNumberEquality);
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
        public int GetDepartmentId()
        {
            return _departmentId;
        }
        public void SetDepartmentId(int departmentId)
        {
            _departmentId = departmentId;
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
                int departmentId = rdr.GetInt32(3);
                Course newCourse = new Course(courseName, courseNumber, departmentId, courseId);
                allCourses.Add(newCourse);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allCourses;
        }


        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO courses (name, course_number, department_id) OUTPUT INSERTED.id VALUES (@Name, @CourseNumber, @DepartmentId);", conn);

            cmd.Parameters.Add(new SqlParameter("@Name", this.GetName()));
            cmd.Parameters.Add(new SqlParameter("@CourseNumber", this.GetCourseNumber()));
            cmd.Parameters.Add(new SqlParameter("@DepartmentId", this.GetDepartmentId()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public void Add(int studentId)
        {
            int majorCourse = 0;
            SqlConnection conn = DB.Connection();
            conn.Open();

            if(this.GetDepartmentId() == Student.Find(studentId).GetDepartmentId())
            {
                majorCourse = 1;
            }

            SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (courses_id, students_id, completed, major) VALUES (@CourseId, @StudentId, @Completed, @Major);", conn);

            cmd.Parameters.Add(new SqlParameter("@CourseId", this.GetId()));
            cmd.Parameters.Add(new SqlParameter("@StudentId", studentId.ToString()));
            cmd.Parameters.Add(new SqlParameter("@Completed", "0"));
            cmd.Parameters.Add(new SqlParameter("@Major", majorCourse));

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

        public int GetMajorCourse(int studentId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT major FROM courses_students WHERE courses_id = @CoursesId AND students_id = @StudentsId;", conn);

            cmd.Parameters.Add(new SqlParameter("@CoursesId", this.GetId()));
            cmd.Parameters.Add(new SqlParameter("@StudentsId", studentId.ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            int majorCourse = 0;

            while(rdr.Read())
            {
                majorCourse = rdr.GetByte(0);
            }

            DB.CloseSqlConnection(rdr, conn);

            return majorCourse;
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
                int departmentId = rdr.GetInt32(3);
                Student newStudent = new Student(studentName, enrollmentDate, departmentId, studentId);
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
            int departmentId = 0;

            while(rdr.Read())
            {
                foundCourseId = rdr.GetInt32(0);
                courseName = rdr.GetString(1);
                courseNumber = rdr.GetString(2);
                departmentId = rdr.GetInt32(3);
            }

            Course foundCourse = new Course(courseName, courseNumber, departmentId, foundCourseId);

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

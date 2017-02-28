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

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }

            return allCourses;
        }

        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM courses;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}

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
                string studentEnrollmentDate = rdr.GetString(2);
                Student newStudent = new Student(studentName, studentEnrollmentDate, studentId);
                allStudents.Add(newStudent);
            }

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }

            return allStudents;
        }

        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM students;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}

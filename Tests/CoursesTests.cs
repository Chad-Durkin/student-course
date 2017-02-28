using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Registrar
{
    public class CourseTest : IDisposable
    {
        public CourseTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=registrar_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Test_CoursesEmptyAtFirst()
        {
            //Arrange, Act
            int result = Course.GetAll().Count;

            //Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Test_Save_AssignsIdToCourseObject()
        {
            //Arrange
            Course testCourse = new Course("English", "ENGL120");
            testCourse.Save();

            //Act
            Course savedCourse = Course.GetAll()[0];
            Console.WriteLine(Course.GetAll().Count);

            int result = savedCourse.GetId();
            int testId = testCourse.GetId();

            //Assert
            Assert.Equal(testId, result);
        }

        [Fact]
        public void Test_Save()
        {
            //Arrange
            Course testCourse = new Course("English", "ENGL120");
            testCourse.Save();

            //Act
            List<Course> result = Course.GetAll();
            List<Course> testList = new List<Course>{testCourse};

            //Assert
            Assert.Equal(testList, result);
        }

        [Fact]
        public void Test_Add_AssignsStudentToACourse()
        {
            //Arrange
            Course testCourse = new Course("English", "ENGL120");
            testCourse.Save();
            Student testStudent = new Student("Britton", "2010-09-01");
            testStudent.Save();

            //Act
            testCourse.Add(testStudent.GetId());
            List<Student> allStudents = testCourse.GetStudents();
            List<Student> result = new List<Student> {testStudent};

            //Assert
            Assert.Equal(result, allStudents);
        }

        [Fact]
        public void Test_FindFindsCourseInDatabase()
        {
            //Arrange
            Course testCourse = new Course("English", "ENGL120");
            testCourse.Save();

            //Act
            Course result = Course.Find(testCourse.GetId());

            //Assert
            Assert.Equal(testCourse, result);
        }


        public void Dispose()
        {
            Student.DeleteAll();
            Course.DeleteAll();
        }
    }
}

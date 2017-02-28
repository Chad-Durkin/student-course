using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Registrar
{
    public class StudentTest : IDisposable
    {
        public StudentTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=registrar_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Test_CitiesEmptyAtFirst()
        {
            //Arrange, Act
            int result = Student.GetAll().Count;

            //Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Test_Save_AssignsIdToStudentObject()
        {
            //Arrange
            Student testStudent = new Student("Britton", "2010-09-01");
            testStudent.Save();

            //Act
            Student savedStudent = Student.GetAll()[0];

            int result = savedStudent.GetId();
            int testId = testStudent.GetId();

            //Assert
            Assert.Equal(testId, result);
        }

        [Fact]
        public void Test_Save()
        {
            //Arrange
            Student testStudent = new Student("Britton", "2010-09-01");
            testStudent.Save();

            //Act
            List<Student> result = Student.GetAll();
            List<Student> testList = new List<Student>{testStudent};

            //Assert
            Assert.Equal(testList, result);
        }

        [Fact]
        public void Test_Add_AssignsCourseToAStudent()
        {
            //Arrange
            Course testCourse = new Course("English", "ENGL120");
            testCourse.Save();
            Student testStudent = new Student("Britton", "2010-09-01");
            testStudent.Save();

            //Act
            testStudent.Add(testCourse.GetId());
            List<Course> allCourses = testStudent.GetCourses();
            List<Course> result = new List<Course>{testCourse};

            //Assert
            Assert.Equal(result, allCourses);
        }

        public void Dispose()
        {
            Student.DeleteAll();
            Course.DeleteAll();
        }
    }
}

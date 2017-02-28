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

        [Fact]
        public void Test_DeleteCourse_DeleteCourseFromDatabase()
        {
            //Arrange
            Course testCourse1 = new Course("English", "ENGL120");
            testCourse1.Save();
            Course testCourse2 = new Course("Math", "MATH101");
            testCourse2.Save();

            //Act
            testCourse2.Delete();

            List<Course> allCourses = Course.GetAll();
            List<Course> expected = new List<Course>{testCourse1};

            //Assert
            Assert.Equal(expected, allCourses);
        }

        [Fact]
        public void Test_GetCompleted_ReturnIfCourseIsCompleted()
        {
            //Arrange
            Course testCourse1 = new Course("English", "ENGL120");
            testCourse1.Save();
            Course testCourse2 = new Course("Math", "MATH101");
            testCourse2.Save();
            Student testStudent = new Student("Britton", "2010-09-01");
            testStudent.Save();

            //Act
            testStudent.Add(testCourse1.GetId());
            testStudent.Add(testCourse2.GetId());
            int result = testStudent.GetCompleted(testCourse1.GetId());
            int expected = 0;

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_UpdateCompleted_ReturnIfCourseCompletedIsUpdated()
        {
            //Arrange
            Course testCourse1 = new Course("English", "ENGL120");
            testCourse1.Save();
            Course testCourse2 = new Course("Math", "MATH101");
            testCourse2.Save();
            Student testStudent = new Student("Britton", "2010-09-01");
            testStudent.Save();

            //Act
            testStudent.Add(testCourse1.GetId());
            testStudent.Add(testCourse2.GetId());
            testStudent.UpdateCompleted(testCourse1.GetId());
            int result = testStudent.GetCompleted(testCourse1.GetId());
            int expected = 1;

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_GetMajorCourse_ReturnIfCourseIsMajorRelated()
        {
            //Arrange
            Department testDepartment = new Department("English");
            testDepartment.Save();
            Course testCourse = new Course("English", "ENGL120", testDepartment.GetId());
            testCourse.Save();
            Student testStudent = new Student("Britton", "2010-09-01", testDepartment.GetId());
            testStudent.Save();

            //Act
            testCourse.Add(testStudent.GetId());
            int result = testCourse.GetMajorCourse(testStudent.GetId());
            int expected = 1;

            //Assert
            Assert.Equal(expected, result);
        }

        public void Dispose()
        {
            Student.DeleteAll();
            Department.DeleteAll();
            Course.DeleteAll();
        }
    }
}

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


        [Fact]
        public void Test_FindFindsStudentsInDatabase()
        {
            //Arrange
            Student testStudent = new Student("Britton", "2010-09-01");
            testStudent.Save();

            //Act
            Student result = Student.Find(testStudent.GetId());

            //Assert
            Assert.Equal(testStudent, result);
        }

        [Fact]
        public void Test_DeleteStudent_DeleteStudentFromDatabase()
        {
            //Arrange
            Student student1 = new Student("Britton", "2010-09-01");
            student1.Save();

            Student student2 = new Student("Melanie", "2011-01-01");
            student2.Save();

            //Act
            student1.Delete();

            List<Student> allStudents = Student.GetAll();
            List<Student> expected = new List<Student>{student2};

            //Assert
            Assert.Equal(expected, allStudents);
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
            int result = testCourse1.GetCompleted(testStudent.GetId());
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
            testCourse1.UpdateCompleted(testStudent.GetId());
            int result = testCourse1.GetCompleted(testStudent.GetId());
            int expected = 1;

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_GetMajorCourse_ReturnIfStudentsIsMajorRelated()
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
            int result = testStudent.GetMajorCourse(testCourse.GetId());
            int expected = 1;

            //Assert
            Assert.Equal(expected, result);
        }

        public void Dispose()
        {
            Student.DeleteAll();
            Course.DeleteAll();
            Department.DeleteAll();
        }
    }
}

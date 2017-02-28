using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Registrar
{
    public class DepartmentTest : IDisposable
    {
        public DepartmentTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=registrar_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Test_DepartmentsEmptyAtFirst()
        {
            //Arrange, Act
            int result = Department.GetAll().Count;

            //Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Test_Save_AssignsIdToDepartmentObject()
        {
            //Arrange
            Department testDepartment = new Department("English");
            testDepartment.Save();

            //Act
            Department savedDepartment = Department.GetAll()[0];

            int result = savedDepartment.GetId();
            int testId = testDepartment.GetId();

            //Assert
            Assert.Equal(testId, result);
        }

        [Fact]
        public void Test_Save()
        {
            //Arrange
            Department testDepartment = new Department("English");
            testDepartment.Save();

            //Act
            List<Department> result = Department.GetAll();
            List<Department> testList = new List<Department>{testDepartment};

            //Assert
            Assert.Equal(testList, result);
        }

        public void Dispose()
        {
            Student.DeleteAll();
            Department.DeleteAll();
            Course.DeleteAll();
        }
    }
}

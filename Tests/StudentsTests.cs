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
        
        public void Dispose()
        {
          Student.DeleteAll();
          Course.DeleteAll();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Module05Exercise01.Model;
using MySql.Data.MySqlClient;
using System.Text;
using System.Data;
using Module0Exercise01.Services;

namespace Module05Exercise01.Services
{
    public class EmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            var employeeList = new List<Employee>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Retrieve Data
                var cmd = new MySqlCommand("SELECT * FROM tblEmployee", conn); // Updated table name

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"), // Updated to EmployeeID
                            Name = reader.GetString("Name"),
                            Address = reader.GetString("Address"), // Added Address
                            Email = reader.GetString("Email"),     // Updated to Email
                            ContactNo = reader.GetString("ContactNo")
                        });
                    }
                }
            }
            return employeeList;
        }
    }
}
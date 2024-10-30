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
            var employeeList = new List<Employee>(); // Changed to employeeList
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Retrieve Data
                var cmd = new MySqlCommand("SELECT * FROM tblEmployee", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"), // Assuming EmployeeID is the primary key
                            Name = reader.GetString("Name"),
                            Address = reader.GetString("Address"), // Added Address
                            Email = reader.GetString("Email"),     // Added Email
                            ContactNo = reader.GetString("ContactNo")
                        });
                    }
                }
            }
            return employeeList;
        }

        public async Task<bool> AddEmployeeAsync(Employee newEmployee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("INSERT INTO tblEmployee (Name, Address, Email, ContactNo) VALUES (@Name, @Address, @Email, @ContactNo)", conn);
                    cmd.Parameters.AddWithValue("@Name", newEmployee.Name);
                    cmd.Parameters.AddWithValue("@Address", newEmployee.Address); // Added Address
                    cmd.Parameters.AddWithValue("@Email", newEmployee.Email);     // Added Email
                    cmd.Parameters.AddWithValue("@ContactNo", newEmployee.ContactNo);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding employee record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM tblEmployee WHERE EmployeeID = @EmployeeID", conn);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting employee record: {ex.Message}");
                return false;
            }
        }
    }
}
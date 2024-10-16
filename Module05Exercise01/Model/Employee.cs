using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module05Exercise01.Model
{
    public class Employee
    {
        public int EmployeeID { get; set; }  // Changed to EmployeeID
        public string Name { get; set; }
        public string Address { get; set; }   // Added Address
        public string Email { get; set; }     // Corrected to Email
        public string ContactNo { get; set; }
    }
}
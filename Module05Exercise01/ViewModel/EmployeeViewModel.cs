using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using Module05Exercise01.Model; // Updated namespace
using Module05Exercise01.Services;

namespace Module05Exercise01.ViewModel
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeService _employeeService;
        public ObservableCollection<Employee> EmployeeList { get; set; }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private Employee _selectedEmployee; // Renamed from Personal
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                if (_selectedEmployee != null)
                {
                    NewEmployeeName = _selectedEmployee.Name;
                    NewEmployeeAddress = _selectedEmployee.Address; // Added
                    NewEmployeeEmail = _selectedEmployee.Email;     // Added
                    NewEmployeeContactNo = _selectedEmployee.ContactNo;
                    IsEmployeeSelected = true;
                }
                else
                {
                    IsEmployeeSelected = false;
                }
                OnPropertyChanged();
                ((Command)DeleteEmployeeCommand).ChangeCanExecute(); // Notify command of change
            }
        }

        private bool _isEmployeeSelected; // Renamed from IsPersonSelected
        public bool IsEmployeeSelected
        {
            get => _isEmployeeSelected;
            set
            {
                _isEmployeeSelected = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        // New Employee Fields
        private string _newEmployeeName;
        public string NewEmployeeName
        {
            get => _newEmployeeName;
            set
            {
                _newEmployeeName = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeAddress; // Added
        public string NewEmployeeAddress
        {
            get => _newEmployeeAddress;
            set
            {
                _newEmployeeAddress = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeEmail; // Added
        public string NewEmployeeEmail
        {
            get => _newEmployeeEmail;
            set
            {
                _newEmployeeEmail = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeContactNo;
        public string NewEmployeeContactNo
        {
            get => _newEmployeeContactNo;
            set
            {
                _newEmployeeContactNo = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand SelectedEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }

        public EmployeeViewModel()
        {
            _employeeService = new EmployeeService();
            EmployeeList = new ObservableCollection<Employee>();
            LoadDataCommand = new Command(async () => await LoadData());
            AddEmployeeCommand = new Command(async () => await AddEmployee());
            SelectedEmployeeCommand = new Command<Employee>(employee => SelectedEmployee = employee);
            DeleteEmployeeCommand = new Command(async () => await DeleteEmployee(),
                                                () => SelectedEmployee != null);

            LoadData();
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading employee data...";
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                EmployeeList.Clear();
                foreach (var employee in employees)
                {
                    EmployeeList.Add(employee);
                }
                StatusMessage = "Data loaded successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddEmployee()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewEmployeeName) ||
                string.IsNullOrWhiteSpace(NewEmployeeAddress) ||
                string.IsNullOrWhiteSpace(NewEmployeeEmail) ||
                string.IsNullOrWhiteSpace(NewEmployeeContactNo))
            {
                StatusMessage = "Please fill in all fields before adding";
                return;
            }
            IsBusy = true;
            StatusMessage = "Adding new employee...";

            try
            {
                var newEmployee = new Employee
                {
                    Name = NewEmployeeName,
                    Address = NewEmployeeAddress, // Added
                    Email = NewEmployeeEmail,     // Added
                    ContactNo = NewEmployeeContactNo
                };
                var isSuccess = await _employeeService.AddEmployeeAsync(newEmployee);
                if (isSuccess)
                {
                    NewEmployeeName = string.Empty;
                    NewEmployeeAddress = string.Empty; // Added
                    NewEmployeeEmail = string.Empty;    // Added
                    NewEmployeeContactNo = string.Empty;
                    StatusMessage = "New employee added successfully";
                }
                else
                {
                    StatusMessage = "Failed to add the new employee";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed adding employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                await LoadData();
            }
        }

        private async Task DeleteEmployee()
        {
            if (SelectedEmployee == null) return;
            var answer = await Application.Current.MainPage.DisplayAlert
                ("Confirm Delete", $"Are you sure you want to delete {SelectedEmployee.Name}?",
                "Yes", "No");

            if (!answer) return;

            IsBusy = true;
            StatusMessage = "Deleting employee...";

            try
            {
                var success = await _employeeService.DeleteEmployeeAsync(SelectedEmployee.EmployeeID); // Changed to EmployeeID
                StatusMessage = success ? "Employee deleted successfully" : "Failed to delete employee";

                if (success)
                {
                    EmployeeList.Remove(SelectedEmployee);
                    SelectedEmployee = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

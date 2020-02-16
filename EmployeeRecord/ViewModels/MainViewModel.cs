using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EmployeeRecord.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace EmployeeRecord.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields and Properties

        private ObservableCollection<Employee> _employeeInfo;
        public ObservableCollection<Employee> EmployeeInfo
        {
            get { return _employeeInfo; }
            set
            {
                _employeeInfo = value;
                this.RaisePropertyChanged("EmployeeInfo");
            }
        }

        private int _empId;
        public int EmpID
        {
            get
            {
                _empId = EmployeeInfo.Max(e => e.EmpID) + 1;
                return _empId;
            }
            set
            {
                _empId = value;
                this.RaisePropertyChanged("EmpID");
            }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                this.RaisePropertyChanged("FirstName");
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                this.RaisePropertyChanged("LastName");
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                this.RaisePropertyChanged("Email");
            }
        }

        private DateTime _dob;
        public DateTime DOB
        {
            get { _dob = DateTime.Today; return _dob; }
            set
            {
                _dob = value;
                this.RaisePropertyChanged("DOB");
            }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                this.RaisePropertyChanged("Phone");
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                this.RaisePropertyChanged("SelectedEmployee");
            }
        }

        private DateTime _workDate;
        public DateTime WorkDate
        {
            get { _workDate = DateTime.Today; return _workDate; }
            set
            {
                _workDate = value;
                this.RaisePropertyChanged("WorkDate");
            }
        }

        private double _hours;
        public double Hours
        {
            get { return _hours; }
            set
            {
                _hours = value;
                this.RaisePropertyChanged("Hours");
            }
        }







        #endregion

        #region Commands

        private RelayCommand<MainWindow> _windowMinCommand;
        public RelayCommand<MainWindow> WindowMinCommand
        {
            get { if (_windowMinCommand == null) _windowMinCommand = new RelayCommand<MainWindow>(w => WindowMinCommandExecutor(w)); return _windowMinCommand; }
            set { _windowMinCommand = value; }
        }


        private RelayCommand<MainWindow> _windowMaxCommand;
        public RelayCommand<MainWindow> WindowMaxCommand
        {
            get { if (_windowMaxCommand == null) _windowMaxCommand = new RelayCommand<MainWindow>(w => WindowMaxCommandExecutor(w)); return _windowMaxCommand; }
            set { _windowMaxCommand = value; }
        }


        private RelayCommand _windowCloseCommand;
        public RelayCommand WindowCloseCommand
        {
            get { if (_windowCloseCommand == null) _windowCloseCommand = new RelayCommand(WindowCloseCommandExecutor); return _windowCloseCommand; }
            set { _windowCloseCommand = value; }
        }


        private RelayCommand _saveEmpCommand;
        public RelayCommand SaveEmpCommand
        {
            get { if (_saveEmpCommand == null) _saveEmpCommand = new RelayCommand(SaveEmpCommandExecutor); return _saveEmpCommand; }
            set { _saveEmpCommand = value; }
        }


        private RelayCommand _clearCommand;
        public RelayCommand ClearCommand
        {
            get { if (_clearCommand == null) _clearCommand = new RelayCommand(ClearCommandExecutor); return _clearCommand; }
            set { _clearCommand = value; }
        }


        private RelayCommand<Employee> _selectEmployeeCommand;
        public RelayCommand<Employee> SelectEmployeeCommand
        {
            get { if (_selectEmployeeCommand == null) _selectEmployeeCommand = new RelayCommand<Employee>(e => SelectEmployeeCommandExecutor(e)); return _selectEmployeeCommand; }
            set { _selectEmployeeCommand = value; }
        }


        private RelayCommand _saveHoursCommand;
        public RelayCommand SaveHoursCommand
        {
            get { if (_saveHoursCommand == null) _saveHoursCommand = new RelayCommand(SaveHoursCommandExecutor); return _saveHoursCommand; }
            set { _saveHoursCommand = value; }
        }


        private RelayCommand<Employee> _deleteRecordCommand;

        public RelayCommand<Employee> DeleteRecordCommand
        {
            get { if (_deleteRecordCommand == null) _deleteRecordCommand = new RelayCommand<Employee>(e => DeleteRecordCommandExecutor(e)); return _deleteRecordCommand; }
            set { _deleteRecordCommand = value; }
        }


        #endregion


        public MainViewModel()
        {
            EmployeeInfo = GetEmployees();
        }

        #region CommandExecutors

        private void WindowMinCommandExecutor(MainWindow window)
        {
            window.WindowState = WindowState.Minimized;
        }
        private void WindowMaxCommandExecutor(MainWindow window)
        {
            if (window.WindowState == WindowState.Normal)
                window.WindowState = WindowState.Maximized;
            else
                window.WindowState = WindowState.Normal;
        }
        private void WindowCloseCommandExecutor()
        {
            Environment.Exit(Environment.ExitCode);
        }
        private void SaveEmpCommandExecutor()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Phone))
                return;
            var employee = new Employee() { EmpID = EmpID, FirstName = FirstName, LastName = LastName, Email = Email, DOB = DOB, Phone = Phone };
            EmployeeInfo.Add(employee);
            using (var dbContext = new EmployeeWorkingHoursEntities())
            {
                dbContext.Employees.Add(employee);
                dbContext.SaveChanges();
            }
            ClearCommandExecutor();
            Messenger.Default.Send(employee, "Expand");
        }
        private void ClearCommandExecutor()
        {
            FirstName = "";
            LastName = "";
            Email = "";
            DOB = DateTime.MinValue;
            Phone = "";
        }
        private void SelectEmployeeCommandExecutor(Employee employee)
        {
            this.SelectedEmployee = employee;
            Messenger.Default.Send(employee, "Expand");
        }
        private void SaveHoursCommandExecutor()
        {
            using (var dbContext = new EmployeeWorkingHoursEntities())
            {
                var empHour = new EmpWorkHour() { EmpID = SelectedEmployee.EmpID, WorkDate = WorkDate, Hours = Hours };
                dbContext.EmpWorkHours.Add(empHour);
                dbContext.SaveChanges();
            }
        }

        private void DeleteRecordCommandExecutor(Employee emp)
        {
            EmployeeInfo.Remove(SelectedEmployee);
            using (var dbContext = new EmployeeWorkingHoursEntities())
            {
                var employee = dbContext.Employees.First(e => e.EmpID == SelectedEmployee.EmpID);
                var employeeHours = dbContext.EmpWorkHours.Where(e => e.EmpID == SelectedEmployee.EmpID).ToList();
                dbContext.Employees.Remove(employee);
                dbContext.EmpWorkHours.RemoveRange(employeeHours);
                dbContext.SaveChanges();
            }
            Messenger.Default.Send(emp, "Expand");
        }


        #endregion

        #region Methods

        private ObservableCollection<Employee> GetEmployees()
        {
            using (var dbContext = new EmployeeWorkingHoursEntities())
            {
                ObservableCollection<Employee> employees = new ObservableCollection<Employee>();
                var record = dbContext.Employees;
                foreach (var r in record)
                    employees.Add(r);
                return employees;
            }
        }

        #endregion
    }
}

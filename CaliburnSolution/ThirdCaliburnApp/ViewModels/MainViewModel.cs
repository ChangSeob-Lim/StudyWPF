using Caliburn.Micro;
using MvvmDialogs;
using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using ThirdCaliburnApp.Models;

namespace ThirdCaliburnApp.ViewModels
{
    public class MainViewModel : Conductor<object>, IHaveDisplayName
    {
        #region 속성영역

        readonly IWindowManager windowManager;
        readonly IDialogService dialogService;

        EmployeesModel employeesModel;

        int id;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
                //NotifyOfPropertyChange(() => CanSaveEmployee);
                NotifyOfPropertyChange(() => CanDeleteEmployee);
            }
        }

        string empName;
        public string EmpName
        {
            get
            {
                return empName;
            }

            set
            {
                empName = value;
                NotifyOfPropertyChange(() => EmpName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        decimal salary;
        public decimal Salary
        {
            get
            {
                return salary;
            }

            set
            {
                salary = value;
                NotifyOfPropertyChange(() => Salary);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        string deptName;
        public string DeptName
        {
            get
            {
                return deptName;
            }

            set
            {
                deptName = value;
                NotifyOfPropertyChange(() => DeptName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        string destination;
        public string Destination
        {
            get
            {
                return destination;
            }

            set
            {
                destination = value;
                NotifyOfPropertyChange(() => destination);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        // 전체 Employees 리스트
        public BindableCollection<EmployeesModel> employees;
        public BindableCollection<EmployeesModel> Employees
        {
            get => employees;
            set
            {
                employees = value;
                NotifyOfPropertyChange(() => Employees);
            }
        }
        // 리스트 중 선택된 하나의 Employee
        EmployeesModel selectedEmployee;
        public EmployeesModel SelectedEmployee
        {
            get
            {
                return selectedEmployee;
            }
            set
            {
                selectedEmployee = value;

                if(value != null) // value가 null 값이라면 값이 없으므로 널 참조
                {
                    Id = value.Id;
                    EmpName = value.EmpName;
                    Salary = value.Salary;
                    DeptName = value.DeptName;
                    Destination = value.Destination;
                }

                NotifyOfPropertyChange(() => SelectedEmployee);
                //NotifyOfPropertyChange(() => Id);
                //NotifyOfPropertyChange(() => EmpName);
                //NotifyOfPropertyChange(() => Salary);
                //NotifyOfPropertyChange(() => DeptName);
                //NotifyOfPropertyChange(() => Destination);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        #endregion

        #region 생성자

        //public MainViewModel()
        //{
        //    //GetEmployees();
        //}

        public MainViewModel(IWindowManager windowManager, IDialogService naviteDialogService)
        {
            this.windowManager = windowManager;
            this.dialogService = dialogService;

            //GetEmployees()
        }



        #endregion

        public void GetEmployees()
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTRING))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(EmployeesTbl.SELECT_EMPLOYEES, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Employees = new BindableCollection<EmployeesModel>();
                while (reader.Read())
                {
                    var temp = new EmployeesModel
                    {
                        Id = (int)reader["id"],
                        EmpName = reader["EmpName"].ToString(),
                        Salary = (decimal)reader["Salary"],
                        DeptName = reader["DeptName"].ToString(),
                        Destination = reader["Destination"].ToString()
                    };
                    Employees.Add(temp);
                }
            }
        }

        public bool CanSaveEmployee
        {
            get => (!string.IsNullOrEmpty(EmpName)) && (Salary >= 0) &&
                   (!string.IsNullOrEmpty(DeptName)) && (!string.IsNullOrEmpty(Destination));

            //get => !(string.IsNullOrEmpty(EmpName)) || (Salary >= 0) ||
            //        (string.IsNullOrEmpty(deptName)) || (string.IsNullOrEmpty(Destination));
        }

        public void SaveEmployee()
        {
            int resultRow = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTRING))
                {
                    conn.Open();
                    //MySqlCommand cmd;
                    //if (Id == 0) 
                    //    cmd = new MySqlCommand(EmployeesTbl.INSERT_EMPLOYEE, conn);
                    //else
                    //    cmd = new MySqlCommand(EmployeesTbl.UPDATE_EMPLOYEE, conn);

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    //if (Id == 0)
                    //    cmd.CommandText = EmployeesTbl.INSERT_EMPLOYEE;
                    //else
                    //    cmd.CommandText = EmployeesTbl.UPDATE_EMPLOYEE;
                    cmd.CommandText = (Id == 0) ? EmployeesTbl.INSERT_EMPLOYEE : EmployeesTbl.UPDATE_EMPLOYEE;

                    MySqlParameter paramEmpName = new MySqlParameter("@EmpName", MySqlDbType.VarChar, 45);
                    paramEmpName.Value = EmpName;
                    cmd.Parameters.Add(paramEmpName);

                    MySqlParameter paramSalary = new MySqlParameter("@Salary", MySqlDbType.Decimal);
                    paramSalary.Value = Salary;
                    cmd.Parameters.Add(paramSalary);

                    MySqlParameter paramDeptName = new MySqlParameter("@DeptName", MySqlDbType.VarChar, 45);
                    paramDeptName.Value = DeptName;
                    cmd.Parameters.Add(paramDeptName);

                    MySqlParameter paramDestination = new MySqlParameter("@Destination", MySqlDbType.VarChar, 45);
                    paramDestination.Value = Destination;
                    cmd.Parameters.Add(paramDestination);

                    if (Id != 0)
                    {
                        MySqlParameter paramid = new MySqlParameter("@id", MySqlDbType.Int32);
                        paramid.Value = Id;
                        cmd.Parameters.Add(paramid);
                    }

                    resultRow = cmd.ExecuteNonQuery();

                    if (resultRow > 0)
                    {
                        //MessageBox.Show("저장 완료!");
                        DialogViewModel dialogVM = new DialogViewModel();
                        dialogVM.DisplayName = "저장되었습니다!";
                        bool? success = windowManager.ShowDialog(dialogVM);

                        GetEmployees();
                    }
                }
            }
            catch (Exception ex)
            {
                DialogViewModel dialogVM = new DialogViewModel();
                dialogVM.DisplayName = $"Error: {ex.Message}";
                bool? success = windowManager.ShowDialog(dialogVM);
            }
        }

        public void NewEmployee()
        {
            Id = 0;
            EmpName = string.Empty;
            Salary = 0;
            DeptName = string.Empty;
            Destination = string.Empty;
        }

        public bool CanDeleteEmployee
        {
            get => !(id == 0);
        }

        public void DeleteEmployee()
        {
            int resultRow = 0;

            using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTRING))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = EmployeesTbl.DELETE_EMPLOYEE;

                MySqlParameter paramid = new MySqlParameter("@id", MySqlDbType.Int32);
                paramid.Value = Id;
                cmd.Parameters.Add(paramid);

                resultRow = cmd.ExecuteNonQuery();

                if (resultRow > 0)
                {
                    //MessageBox.Show("삭제 완료!");
                    DialogViewModel dialogVM = new DialogViewModel();
                    dialogVM.DisplayName = "삭제되었습니다!"; // 글자 변경 가능!
                    bool? success = windowManager.ShowDialog(dialogVM);

                    NewEmployee();
                    GetEmployees();
                }
            }
        }
    }
}

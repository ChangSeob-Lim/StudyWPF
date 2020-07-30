using Caliburn.Micro;
using MySql.Data.MySqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using ThirdCaliburnApp.Models;

namespace ThirdCaliburnApp.ViewModels
{
    public class MainViewModel : Conductor<object>, IHaveDisplayName
    {
        #region 속성영역

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
                NotifyOfPropertyChange(() => CanSaveEmployee);
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

                Id = value.Id;
                EmpName = value.EmpName;
                Salary = value.Salary;
                DeptName = value.DeptName;
                Destination = value.Destination;

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
        public MainViewModel()
        {
            //GetEmployees();
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
            get
            {
                return (!string.IsNullOrEmpty(EmpName)) && 
                       (Salary >= 0) && 
                       (!string.IsNullOrEmpty(DeptName)) && 
                       (!string.IsNullOrEmpty(Destination));
            }
        }

        public void SaveEmployee()
        {
            int resultRow = 0;

            using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTRING))
            {
                conn.Open();
                MySqlCommand cmd;
                if (Id == 0) 
                    cmd = new MySqlCommand(EmployeesTbl.INSERT_EMPLOYEE, conn);
                else
                    cmd = new MySqlCommand(EmployeesTbl.UPDATE_EMPLOYEE, conn);

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

                MySqlParameter paramid = new MySqlParameter("@id", MySqlDbType.VarChar, 45);
                paramid.Value = Id;
                cmd.Parameters.Add(paramid);

                resultRow = cmd.ExecuteNonQuery();

                if(resultRow > 0)
                {
                    MessageBox.Show("저장 완료!");

                    GetEmployees();
                }
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
    }
}

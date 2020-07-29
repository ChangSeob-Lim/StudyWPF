using Caliburn.Micro;
using MySql.Data.MySqlClient;
using SecondCaliburnApp.Helpers;
using SecondCaliburnApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SecondCaliburnApp.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHaveDisplayName
    {
        public string DisplayName { get; set; }

        string firstName;
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                NotifyOfPropertyChange(() => FullName); // 변경한 곳에 대해 설정해줌
                NotifyOfPropertyChange(() => CanClearName);
            }
        }

        string lastName;
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(() => FullName);
                NotifyOfPropertyChange(() => CanClearName);
            }
        }

        public string FullName
        {
            get => $"{FirstName} {LastName}"; //string.format("{0} {1}", FirstName, LastName);
        }

        public ShellViewModel()
        {
            DisplayName = "Second Caliban App";
            FirstName = "ChangSeob";
            LastName = "Lim";

            People = new BindableCollection<PersonModel>();
            //People.Add(new PersonModel { LastName = "Gates", FirstName = "Bill" });
            //People.Add(new PersonModel { LastName = "Jobs", FirstName = "Steve" });
            //People.Add(new PersonModel { LastName = "Musk", FirstName = "Ellen" });

            InitCombobox();
        }

        private void InitCombobox()
        {
            People.Add(new PersonModel { FirstName = "선택", LastName = "" });

            using (MySqlConnection conn = new MySqlConnection(Commons.STRCONNSTRING))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(Commons.SELECTPEOPLETBLQUERY, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var temp = new PersonModel
                    {
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString()
                    };
                    People.Add(temp);
                }
            }

            SelectedPerson = People.Where(v => v.FirstName.Contains("선택")).First<PersonModel>();
        }

        public BindableCollection<PersonModel> People { get; set; }

        PersonModel selectedPerson;

        public PersonModel SelectedPerson
        {
            get => selectedPerson;
            set
            {
                selectedPerson = value;
                this.LastName = selectedPerson.LastName;
                this.FirstName = selectedPerson.FirstName;
                NotifyOfPropertyChange(() => SelectedPerson);
                NotifyOfPropertyChange(() => CanClearName);
            }
        }

        public void ClearName()
        {
            this.FirstName = this.LastName = string.Empty;

        }

        public bool CanClearName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                    return false;
                else
                    return true;
                //return !(string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName));
            }
        }

        public void LoadPageOne()
        { // UserControl FirstChildView
            ActivateItem(new FirstChildViewModel());
        }

        public void LoadPageTwo()
        { // UserControl SecondChildView
            ActivateItem(new SecondChildViewModel());
        }
    }
}

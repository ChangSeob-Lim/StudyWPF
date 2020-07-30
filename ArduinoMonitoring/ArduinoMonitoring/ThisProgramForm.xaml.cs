using MahApps.Metro.Controls;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace ArduinoMonitoring
{
    /// <summary>
    /// ThisProgramForm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ThisProgramForm : MetroWindow
    {
        public ThisProgramForm()
        {
            InitializeComponent();

            ShowTitleBar = false;
            ShowMaxRestoreButton = false;
            ShowMinButton = false;
            ShowCloseButton = false;

            this.TxtProductName.Text = AssemblyProduct;
            this.TxtVersion.Text = String.Format("버전 {0}", AssemblyVersion);
            this.TxtCopyright.Text = AssemblyCopyright;
            this.TxtCompanyName.Text = AssemblyCompany;
            this.TxtDescription.Text = AssemblyDescription;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region 어셈블리 특성 접근자
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.O))
                BtnOK_Click(sender, new RoutedEventArgs());
        }
    }
}

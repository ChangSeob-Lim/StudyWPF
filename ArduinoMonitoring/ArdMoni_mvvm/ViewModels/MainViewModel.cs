using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArdMoni_mvvm.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public void InfoOpen()
        {
            IWindowManager btninfo = new WindowManager();
            btninfo.ShowDialog(new InfoViewModel(), null, null);
        }
    }
}

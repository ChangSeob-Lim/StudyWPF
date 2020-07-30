using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArdMoni_mvvm.ViewModels
{
    public class InfoViewModel : Conductor<object>
    {
        public InfoViewModel()
        {

        }

        public void BtnClose()
        {
            (GetView() as Window).Close();
        }
    }
}

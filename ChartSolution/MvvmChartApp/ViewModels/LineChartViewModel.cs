using Caliburn.Micro;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmChartApp.ViewModels
{
    class LineChartViewModel : Conductor<object>
    {
        ChartValues<double> lineValues;
        public ChartValues<double> LineValues
        {
            get => lineValues;
            set
            {
                lineValues = value;
                NotifyOfPropertyChange(() => LineValues);
            }
        }

        public LineChartViewModel()
        {
            InitializeChartData(); // Chart 초기화
        }

        private void InitializeChartData()
        {
            LineValues = new ChartValues<double> { 3, 5, 6.7, 12.4, 0, 7, 9, 4.5 };
        }
    }
}

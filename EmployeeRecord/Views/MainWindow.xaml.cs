using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EmployeeRecord.Models;
using EmployeeRecord.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace EmployeeRecord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            this.MouseDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
            Messenger.Default.Register<Employee>(this, "Expand", ExpandRow);
        }

        private void ExpandRow(Employee employee)
        {
            var rows = recordHoursGrid.RowDefinitions;
            rows[0].Height = new GridLength(1, GridUnitType.Star);
        }
    }
}

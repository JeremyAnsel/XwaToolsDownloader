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

namespace XwaToolsLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ToolsDirectory = @".";

        public MainWindow()
        {
            InitializeComponent();

            Tools = Tool.GetToolsList(ToolsDirectory);

            this.DataContext = this;
        }

        public List<Tool> Tools { get; set; }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tool = button.DataContext as Tool;

            try
            {
                tool.Launch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}

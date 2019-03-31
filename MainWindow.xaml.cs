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

namespace PTLive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PoeFetcher pf = new PoeFetcher("Get Fucked Again (PL3226)", 0);
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Progress<FetcherProgressReport> progress = new Progress<FetcherProgressReport>();
            progress.ProgressChanged += Progress_ProgressChanged;
            pf.CompletedSuccessfully += Pf_CompletedSuccessfully;
            pf.InitUpdateData();
        }

        private void Pf_CompletedSuccessfully()
        {
            output1.Dispatcher.BeginInvoke(new Action(() => { output1.Text = "COMPLETED!!!"; }));
        }

        private void Progress_ProgressChanged(object sender, FetcherProgressReport e)
        {
            output1.Text = e.percentage.ToString();
        }
    }
}

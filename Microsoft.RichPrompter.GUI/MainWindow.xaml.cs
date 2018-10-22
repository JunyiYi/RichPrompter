using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;

namespace Microsoft.RichPrompter.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
            };
            dialog.ShowDialog();
            ((Button)sender).CommandParameter = dialog.FileName;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(LoadContent));
        }

        private readonly IList<FlowDocument> documents = new List<FlowDocument>();

        private void LoadContent()
        {
            Cues.Items.Clear();
            documents.Clear();
            foreach (var cue in App.Document.Cues)
            {
                var doc = new FlowDocument();
                foreach (var block in cue.Blocks)
                {
                    var para = new Paragraph();
                    foreach (var inline in block.Inlines)
                    {
                        para.Inlines.Add(new Run(((TextInline)inline).Text));
                    }
                    doc.Blocks.Add(para);
                }
                documents.Add(doc);
                Cues.Items.Add(cue.Header);
            }
            Cues.SelectedIndex = 0;
        }

        private void Cues_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActiveCue.Document = documents[Cues.SelectedIndex];
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private IntPtr prevHWnd = IntPtr.Zero;

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            Cues.SelectedIndex = Math.Max(Cues.SelectedIndex - 1, 0);
            if (prevHWnd != IntPtr.Zero)
            {
                SetForegroundWindow(prevHWnd);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Cues.SelectedIndex = Math.Min(Cues.SelectedIndex + 1, documents.Count - 1);
            if (prevHWnd != IntPtr.Zero)
            {
                SetForegroundWindow(prevHWnd);
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            prevHWnd = GetForegroundWindow();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            prevHWnd = IntPtr.Zero;
        }
    }
}

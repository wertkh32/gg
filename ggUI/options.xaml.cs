using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
namespace ggUI
{
    /// <summary>
    /// Interaction logic for options.xaml
    /// </summary>
    public partial class options : Window
    {
        public Key ShowHotkey, ExitHotkey, ScrollHotkey;
        public bool RunOnStartup;
        private MainWindow winmain;
        public options(MainWindow win)
        {
            winmain = win;
            InitializeComponent();
        }
        public void UpdateValues()
        {
            ShowHotkey = winmain.ShowHotkey;
            ExitHotkey = winmain.ExitHotkey;
            ScrollHotkey = winmain.ScrollHotkey;
            RunOnStartup = winmain.RunOnStartup;

            txtHotkeyShow.Text = ShowHotkey.ToString();
            txtHotkeyExit.Text = ExitHotkey.ToString();
            txtHotkeyScroll.Text = ScrollHotkey.ToString();
            chkStartup.IsChecked = RunOnStartup;

            btnSave.Focus();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            winopt.Top = -SystemParameters.PrimaryScreenHeight * 0.06;
            winopt.Left = winmain.RTBWidth + winmain.RTBLeft + winmain.Left;
            winopt.RenderTransform = new TranslateTransform();
            optgrid.Clip = new RectangleGeometry(new Rect(new Point(0,0), new Point(optgrid.Width, optgrid.Height)), 6, 6);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            OptionsIO optio = new OptionsIO(MainWindow.SettingsFile);
            OptionsObject opt=new OptionsObject();
            opt.RunOnStartup = RunOnStartup;
            opt.ShowHotkey = ShowHotkey;
            opt.ExitHotkey = ExitHotkey;
            opt.ScrollHotkey = ScrollHotkey;
            optio.Write(opt);

            winmain.ShowHotkey = ShowHotkey;
            winmain.ExitHotkey = ExitHotkey;
            winmain.ScrollHotkey = ScrollHotkey;
            winmain.RunOnStartup = RunOnStartup;

            if (RunOnStartup) winmain.CreateShortcut();
            else winmain.DeleteShortcut();
            
            winmain.ShowHideOptions();
        }

        private void txtHotkeyShow_KeyDown(object sender, KeyEventArgs e)
        {
            ShowHotkey = e.SystemKey==Key.None ?  e.Key : e.SystemKey;
            txtHotkeyShow.Text = ShowHotkey.ToString();
            
        }

        private void txtHotkeyExit_KeyDown(object sender, KeyEventArgs e)
        {
            ExitHotkey = e.SystemKey == Key.None ? e.Key : e.SystemKey;
            txtHotkeyExit.Text = ExitHotkey.ToString();
           
        }

        private void txtHotkeyScroll_KeyDown(object sender, KeyEventArgs e)
        {
            ScrollHotkey = e.SystemKey == Key.None ? e.Key : e.SystemKey;
            txtHotkeyScroll.Text = ScrollHotkey.ToString();
            
        }

        private void chkStartup_Checked(object sender, RoutedEventArgs e)
        {
            RunOnStartup = true;
        }

        private void chkStartup_Unchecked(object sender, RoutedEventArgs e)
        {
            RunOnStartup = false;
        }

       
     
    }
    
}

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
using CefSharp;
using CefSharp.Wpf;

namespace ResUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var settings = new CefSettings();
            settings.CachePath = Environment.CurrentDirectory + @"\CEF-Cookies";
            settings.PersistUserPreferences = true;
            settings.PersistSessionCookies = true;
            //settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF";
            CefSharp.Cef.Initialize(settings);

            InitializeComponent();
            //Cef.Initialize(new CefSettings { CachePath = Environment.CurrentDirectory }, false, true);
            //Browser.BrowserSettings.ApplicationCache = CefState.Enabled;
            Browser.Initialized += BrowserOnInitialized;
        }

        private void BrowserOnInitialized(object sender, EventArgs e)
        {
            //Browser.GetCookieManager().SetStoragePath(Environment.CurrentDirectory, true);
        }

        private async void webbrowsertopbuttonclick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Tag is string s)
                {
                    Browser.Load(s);
                    return;
                }

                var cc = Browser.GetCookieManager();
                var t = new TaskCookieVisitor();
                cc.VisitAllCookies(t);
                var res = await t.Task;
            }
        }
    }
}

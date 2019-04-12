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

                var cookieManager = Browser.GetCookieManager();
                var csrf_token = await Browser.EvaluateScriptAsync("window.csrf_token", TimeSpan.FromMilliseconds(2000));
                var cos2 = await cookieManager.VisitAllCookiesAsync();

                var all = cos2.Where(x => !string.IsNullOrEmpty(x.Domain) && x.Domain.Contains("e-vybory.org")).ToList();

                var dic = new Dictionary<string, string>();
                foreach (var cookie in all)
                {
                    dic[cookie.Name] = cookie.Value;
                }

                var us = new UploadService();
                await us.Test(
                    "68"
                    , "681325"
                    , "192"
                    , $"Hello, {DateTime.Now.ToShortTimeString()}"
                    , dic
                    , csrf_token.Result.ToString());
            }
        }
    }
}

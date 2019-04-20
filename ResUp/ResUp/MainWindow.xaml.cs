using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using CefSharp.Wpf;
using SiteClient;

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
            CefSharp.Cef.Initialize(settings);
            InitializeComponent();
            Browser.Initialized += BrowserOnInitialized;
        }

        private void BrowserOnInitialized(object sender, EventArgs e)
        {
            //Browser.GetCookieManager().SetStoragePath(Environment.CurrentDirectory, true);
        }

        private async Task<(bool isOk, string CsrfToken, Dictionary<string, string> Cookies)> GetDataForUpload()
        {
            var isOk = true;
            var csrfToken = "";
            var cookies = new Dictionary<string, string>();

            try
            {
                var cookieManager = Browser.GetCookieManager();
                var csrf_token_raw = await Browser.EvaluateScriptAsync("window.csrf_token", TimeSpan.FromMilliseconds(2000));
                var cos2 = await cookieManager.VisitAllCookiesAsync();

                var all = cos2.Where(x => !string.IsNullOrEmpty(x.Domain) && x.Domain.Contains("e-vybory.org")).ToList();

                foreach (var cookie in all)
                {
                    cookies[cookie.Name] = cookie.Value;
                }

                csrfToken = csrf_token_raw.Result.ToString();

                if (string.IsNullOrEmpty(csrfToken))
                {
                    isOk = false;
                }

                if (!cookies.Any())
                {
                    isOk = false;
                }

            }
            catch (Exception e)
            {
                OnUserError(e.Message);
                isOk = false;
            }

            return (isOk, csrfToken, cookies);
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

                //var us = new DocumentService();
                ////var res = await us.CreateRemoteDocument(
                ////    "68"
                ////    , "65"
                ////    , "12"
                ////    , $"Hello, {DateTime.Now.ToShortTimeString()}"
                ////    , dic
                ////    , csrfToken);

                ////if (res.ІsSuccessful)
                ////{
                ////var uploadFilesRes = await us.UploadToRemoteDocument(res.DocumentId, dic, csrfToken, new[] { "" });
                ////}

                //var uploadFilesRes = await us.UploadToRemoteDocument("987", dic, csrfToken, new[] { @"" });
            }
        }

        private async void SelectWorkingFolder(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Select a Directory",
                Filter = "Directory|*.this.directory",
                FileName = "select",
                RestoreDirectory = true
            };
            string path = null;
            //dialog.InitialDirectory = textbox.Text; // Use current value for initial dir
            // instead of default "Save As"
            // Prevents displaying files
            // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                this.SelectedWorkingPath.Content = $"working folder: '{path}'";
            }
            else
            {
                this.SelectedWorkingPath.Content = "";
            }

            try
            {
                await SelectLocalDocumentsFromFolder(path);
            }
            catch (Exception exception)
            {
                OnUserError(exception.ToString());
            }
        }

        private Document[] _selectedDocs;

        private async Task SelectLocalDocumentsFromFolder(string path)
        {
            _selectedDocs = null;
            Output.Text = "";
            if (string.IsNullOrEmpty(path))
            {
                Output.Text = "select valid directory to continue";
            }

            var us = new DocumentService();
            var docs = await us.GetLocalDocuments(path);

            if (docs?.Any() != true)
            {
                this.SelectedWorkingPath.Content = $"nothing found in the '{path}'.";
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine($"Folder processed. {docs.Length} documents.");

            var hasDocs = docs.Where(x => x.Files?.Any() == true).ToList();
            var docsWithErrors = docs.Where(x => x.HasError).ToList();
            var allSent = hasDocs.Where(x => x.Sent).ToList();
            var toSend = hasDocs.Where(x => !x.Sent & !x.HasError).ToList();

            _selectedDocs = toSend.ToArray();

            sb.AppendLine($"Total documents {docs.Length}");
            sb.AppendLine($"With content {hasDocs.Count}");
            sb.AppendLine($"Loaded with errors {docsWithErrors.Count}");
            sb.AppendLine($"Already sent {allSent.Count}");
            sb.AppendLine($"is going to be sent {toSend.Count}");
            sb.AppendLine("========================================");
            sb.AppendLine($"Loaded with errors:");
            sb.AppendLine(string.Join(",", docsWithErrors.Select(x => x.Name)));
            sb.AppendLine("========================================");
            sb.AppendLine($"Already sent:");
            sb.AppendLine(string.Join(",", allSent.Select(x => x.Name)));
            sb.AppendLine("========================================");
            sb.AppendLine($"To send:");
            sb.AppendLine(string.Join(",", toSend.Select(x => x.Name)));
            sb.AppendLine("========================================");
            Output.Text = sb.ToString();
        }

        private bool _isUploading = false;
        private async void StartUploading(Document[] docs)
        {
            if (_isUploading)
                return;
            _isUploading = true;

            var sb = new StringBuilder(Output.Text);

            Output.Text = "uploading...";

            try
            {
                if (docs?.Any() == false)
                {
                    OnUserError("there are no selected documents");
                    return;
                }

                var d = await GetDataForUpload();

                if (!d.isOk)
                {
                    OnUserError("login first or error, you can't continue");
                    return;
                }

                webbrowsertopbuttonclick(MyDocsButton, null);
                await Task.Delay(2000);
                webbrowsertopbuttonclick(HomeButton, null);
                await Task.Delay(2000);
                webbrowsertopbuttonclick(MyDocsButton, null);
                await Task.Delay(2000);

                //docs = docs.Take(10).ToArray();

                void LogLocal(string s)
                {
                    sb.AppendLine(s);
                    Output.Text = s;
                }

                var ds = new DocumentService();
                sb.AppendLine("========================================");
                sb.AppendLine($"starting...");
                foreach (var document in docs)
                {
                    if (document.Sent)
                    {
                        LogLocal($"{document.Name} - already sent, skipping");
                        continue;
                    }

                    try
                    {
                        document.RegionId = "68";

                        var res = await ds.CreateRemoteDocument(
                            document.RegionId
                            , document.PollingStation
                            , document.DistrictNumber
                            , $"created by upl04d 4u70m470r"
                            , d.Cookies
                            , d.CsrfToken
                            , false);

                        if (res.ІsSuccessful)
                        {
                            var uploadFilesRes = await ds.UploadToRemoteDocument(res.DocumentId, d.Cookies, d.CsrfToken, document.Files);
                            if (uploadFilesRes)
                            {
                                document.MakeDone();
                                LogLocal($"{document.Name} - done");
                            }
                            else
                            {
                                document.MakeError();
                                LogLocal($"{document.Name} - error");
                            }
                        }
                        else
                        {
                            document.MakeError();
                            LogLocal($"{document.Name} - error");
                        }
                    }
                    catch (Exception e)
                    {
                        document.MakeError();
                        LogLocal($"{document.Name} - system error");
                    }
                }

                sb.AppendLine($"all done");
                sb.AppendLine("========================================");
            }
            catch (Exception e)
            {
                OnUserError(e.Message);
            }
            finally
            {
                Output.Text = sb.ToString();
                _isUploading = false;
            }
        }

        private void BeginUploadClick(object sender, RoutedEventArgs e)
        {
            StartUploading(this._selectedDocs);
        }

        private void OnUserError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK);
        }
    }
}

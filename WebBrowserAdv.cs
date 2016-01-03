using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reader_Layer
{
   public class WebBrowserAdv : IDisposable
    {
        protected WebBrowser browser = null;
        private bool complete = false;

        static WebBrowserAdv()
        {
            string executablePath = Environment.GetCommandLineArgs()[0];
            string executableName = System.IO.Path.GetFileName(executablePath);
            
            // set to use ie 10 (http://stackoverflow.com/questions/15829397/webbrowser-using-ie10-c-sharp-winform)
            RegistryKey registrybrowser = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            if (registrybrowser != null)
            { 
                registrybrowser.SetValue(executableName, 0x02710, RegistryValueKind.DWord);
                registrybrowser.Close();
            }

            // set to use ie 10 (http://stackoverflow.com/questions/15829397/webbrowser-using-ie10-c-sharp-winform)
            registrybrowser = Registry.CurrentUser.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            if (registrybrowser != null)
            {
                registrybrowser.SetValue(executableName, 0x02710, RegistryValueKind.DWord);
                registrybrowser.Close();
            }
        }

        public WebBrowserAdv(WebBrowser browser = null)
        {
            this.browser = browser ?? new WebBrowser();
            this.browser.ScriptErrorsSuppressed = true;
            this.browser.DocumentCompleted += browser_DocumentCompleted;
        }

        public Uri Url { get { return browser.Url; } }
        public WebBrowser webBrowser { get { return browser;     } }

        public HtmlAgilityPack.HtmlDocument GetHtmlAgilityPackDocument()
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(this.browser.DocumentText);
            return doc;
        }

        virtual public bool Navigate(int millisecondsTimeout, string url, params object[] args)
        {
            this.complete = false;
            System.Diagnostics.Stopwatch timeout = new System.Diagnostics.Stopwatch();
            timeout.Start();

            browser.Stop();
            System.Threading.Thread.Sleep(500);
            browser.Navigate(string.Format(url, args));

            timeout.Start();
            Application.DoEvents();

            while (!this.complete && timeout.ElapsedMilliseconds < millisecondsTimeout)
            {
                System.Threading.Thread.Sleep(1000);
                Application.DoEvents();
            }

            // Clear remaining script events
            Application.DoEvents();

            return this.complete;
        }

        virtual public bool InvokeScript(int millisecondsTimeout, string script, params object[] args)
        {
            this.complete = false;
            System.Diagnostics.Stopwatch timeout = new System.Diagnostics.Stopwatch();

            //browser.Stop();

            // Clear remaining script events
            Application.DoEvents();

            browser.Document.InvokeScript("execScript", new Object[] { string.Format(script, args), "JavaScript" });

            timeout.Start();
            Application.DoEvents();

            while (!this.complete && timeout.ElapsedMilliseconds < millisecondsTimeout)
            {
                System.Threading.Thread.Sleep(500);
                Application.DoEvents();
            }

            // Clear remaining script events
            Application.DoEvents();

            return this.complete;
        }

        virtual public object InvokeScriptReturnValue(int millisecondsTimeout, string script, params object[] args)
        {
            this.complete = false;
            System.Diagnostics.Stopwatch timeout = new System.Diagnostics.Stopwatch();

            //browser.Stop();

            // Clear remaining script events
            Application.DoEvents();

            string func = "(function() { " + string.Format(script, args) + "})()";
            object returnVal = browser.Document.InvokeScript("eval", new Object[] { func });

            timeout.Start();
            Application.DoEvents();

            while (!this.complete && timeout.ElapsedMilliseconds < millisecondsTimeout)
            {
                System.Threading.Thread.Sleep(500);
                Application.DoEvents();
            }

            // Clear remaining script events
            Application.DoEvents();

            return returnVal;
        }

        public virtual void Dispose()
        {
            browser.Stop();
            browser.Dispose();
        }

        protected void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.complete = true;
        }
    }
}

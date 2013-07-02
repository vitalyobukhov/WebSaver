using System.Threading;
using System.Threading.Tasks;
using Common;
using Scr.Content;
using Scr.Input;
using System;
using System.IO;
using System.Windows.Forms;
using Scr.Window;

namespace Scr
{
    partial class MainForm : Form
    {
        // screensaver startup args
        private readonly StartupArgs startupArgs;

        // temporary extracted web content disposable wrapper
        private ContentContainer content;

        // user activity input tracking
        private InputTracker input;


        public MainForm(StartupArgs startupArgs)
        {
            if (startupArgs == null)
                throw new ArgumentNullException("startupArgs");

            this.startupArgs = startupArgs;

            InitializeComponent();
        }


        private void HandleConfiguration()
        {
            // show config message
            MessageBox.Show(this,
                Localization.ConfigurationMessage, Localization.ConfigurationCaption,
                MessageBoxButtons.OK, MessageBoxIcon.Information, 
                MessageBoxDefaultButton.Button1);
        }

        private bool HandlePreview()
        {
            try
            {
                // dock screensaver for preview
                var parentWindowHandle = new IntPtr(((PreviewArgs)startupArgs).ParentWindowHandle);
                WindowPositioner.DockAsChild(this, parentWindowHandle);

                return true;
            }
            catch
            {
                MessageBox.Show(this,
                    Localization.PreviewMessage, Localization.PreviewCaption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                
                return false;
            }
        }

        private void HandleRun()
        {
            // init tracker for run
            input = new InputTracker((s, e) => Close())
            {
                SynchronizingObject = this,
                Interval = 250,
                Enabled = false
            };
        }

        // handle screensaver startup args
        private bool HandleArgs()
        {
            bool result;

            if (startupArgs is ConfigureArgs)
            {
                HandleConfiguration();
                result = false;
            }
            else if (startupArgs is PreviewArgs)
            {
                result = HandlePreview();
            }
            else
            {
                HandleRun();
                result = true;
            }

            return result;
        }

        // check ie version
        private bool CheckBrowser()
        {
            var result = Browser.Version.Major >= Constants.MinBrowserVersion;

            if (!result)
            {
                MessageBox.Show(this,
                    Localization.BrowserUpdateRequiredMessage, Localization.BrowserUpdateRequiredCaption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0,
                    Localization.BrowserUpdateRequiredUrl);
            }

            return result;
        }

        // try to extract web content into temporary directory
        private bool InitContent()
        {
            try
            {
                content = ContentContainer.Load();

                return true;
            }
            catch (LoadContentException ex)
            {
                MessageBox.Show(this,
                    Localization.LoadContentErrorCaption, ex.FriendlyMessage,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, 
                    MessageBoxDefaultButton.Button1);

                return false;
            }
        }

        // all conditions were met, screensaver can start
        private void InitUI()
        {
            var contentFilePath = Path.Combine(content.ContentPath, Constants.ContentFilename);
            Browser.Url = new Uri(contentFilePath);

            if (startupArgs is RunArgs)
            {
                Cursor.Hide();
                Task.Run(() => { Thread.Sleep(Constants.InputDelay); input.Start(); });
            }

            Show();
        }

        private void MainForm_Load(object sender, EventArgs args)
        {
            Hide();

            if (!HandleArgs() || !CheckBrowser() || !InitContent())
            {
                Close();
                return;
            }

            InitUI();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (content != null)
                    content.Dispose();

                if (input != null)
                    input.Dispose();

                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}

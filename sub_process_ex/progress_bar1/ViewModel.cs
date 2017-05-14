using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;

namespace progress_bar1
{
    class ViewModel : ObservableObject
    {
        BackgroundWorker bgw;

        /// <summary>
        /// ctor
        /// </summary>
        public ViewModel()
        {
            // OBS: denne property-bindingen kan også skje run-time!
            RunUpdateButton = new RelayCommand(new Action<object>(RunUpdate));  //  binder indirekte til 'ShowMessage()'-method, implementert under.
            CancelUpdateButton = new RelayCommand(new Action<object>(CancelUpdate));  

            // BGW:
            bgw = new BackgroundWorker();
            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;
            // Set up the Background Worker Events
            bgw.RunWorkerCompleted += UpdateCompleted; // completed event
            bgw.ProgressChanged += UpdateProgressChanged;

            // Set up the Background Worker Events - 'DoWork' and 'RunWorkerAsync' and 'RunWorkerCompleted' :
            //bgw.DoWork += GetFakeValue;
            bgw.DoWork += RunUpdaterProcess;
        }

        #region Properties

        private ICommand m_RunUpdateButton;
        public ICommand RunUpdateButton       // property som skjuler m_ButtonCommand-variabel --> d.v.s. INDIREKTE binding!
        {
            get
            {
                return m_RunUpdateButton;
            }
            set
            {
                m_RunUpdateButton = value;
            }
        }

        private ICommand m_CancelUpdateButton;
        public ICommand CancelUpdateButton       
        {
            get
            {
                return m_CancelUpdateButton;
            }
            set
            {
                m_CancelUpdateButton = value;
            }
        }

        // Andre properties med notification:

        private float _updateProgress = 0.0F;
        public float updateProgress
        {
            get { return _updateProgress; }
            set
            {
                _updateProgress = value;
                RaisePropertyChangedEvent(nameof(updateProgress));
            }
        }

        // 3 Inter-connected properties
        private bool _status = false;
        public bool status
        {
            get { return _status; }
            set
            {
                _status = value;
                // Ha evt. sjekk av status her - oppdater tekst & farge tilsvarende!
                RaisePropertyChangedEvent(nameof(status));
                RaisePropertyChangedEvent(nameof(statusColor));
                RaisePropertyChangedEvent(nameof(statusText));
            }
        }
        public Brush _statusColor = Brushes.LightBlue; 
        public Brush statusColor
        {
            get
            {
                //return _status ? Brushes.LightGreen : Brushes.Red;    // Koder true=Green, false=Red (bruk evt. 'map')
                return _statusColor;
            }
            set
            {
                _statusColor = value;
                RaisePropertyChangedEvent(nameof(statusColor));
            }
        }
        private string _statusText = string.Empty;
        public string statusText
        {
            get
            {
                return _statusText;
            }
            set
            {
                _statusText = value;
                RaisePropertyChangedEvent(nameof(statusText));
            }
        }

        #endregion

        #region BGW Events
        void UpdateProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            updateProgress = e.ProgressPercentage;    // Or, send value through the 'e'-objects' <Argument> property!
        }

        void UpdateCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                status = false;
                statusText = "Update cancelled!";
                statusColor = Brushes.Red;
            }
            else
            {
                statusText = "Update completed!";
                status = true;
                statusColor = Brushes.Green;
                System.Windows.MessageBox.Show("Update Completed!");    // Or, send value through the 'e'-objects' <Argument> property!
            }
        }
        #endregion

        #region Commands

        // For 'Run' button:
        public void RunUpdate(object obj)
        {
            statusText = "Please wait - update started ...";
            statusColor = Brushes.Yellow;
            status = false;
            bgw.RunWorkerAsync();
        }

        public void CancelUpdate(object obj)
        {
            bgw.CancelAsync();
       }

        #endregion

        #region Methods ('worker(s)')
        private void GetFakeValue(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i <= 100; i++)
            {
                // CancellationPending settes v. 'bgw.CancelAsync()':
                if (bgw.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(100);
            }
        }

        private void RunUpdaterProcess(object sender, DoWorkEventArgs e)
        {
            char[] pbuf = new char[80];

            Process process = new Process();
            process.StartInfo.FileName = "test_app/bin/Debug/test_app.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            Thread.Sleep(100);  // TODO: important - workaround to allow STDOUT to become available first! (or - better - set properties ...)

            int i = 10;
            while (process.HasExited == false)
            {
                // async - won't block GUI ...
                int num = process.StandardOutput.Read();
                char c = (char)num;
                //int numBytes = process.StandardOutput.Read(pbuf, 0, 1);   // 1-by-1 char-buffered!
                //pbuf[numBytes] = '\0';
                //string inp = new string(pbuf);
                //Console.WriteLine("Got input from subproc: " + inp + "\n");
                // Update BGW
                // if (inp==".")
                Console.WriteLine(string.Format("Got input from subproc: {0}\n", c));
                if ( c == '.')
                {
                    (sender as BackgroundWorker).ReportProgress(i);
                    i += 10;
                }               
            }

            process.WaitForExit();           // *SKAL* være safe - men OBS: chars KAN ankomme senere!!
            process.Close();

        }

        #endregion
    }

}


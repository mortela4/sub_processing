using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;

namespace progress_bar1
{
    class TestModel : ObservableObject
    {
        BackgroundWorker bgw;

        /// <summary>
        /// ctor
        /// </summary>
        public TestModel()
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
            bgw.DoWork += GetFakeValue;
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
        #endregion
    }

}


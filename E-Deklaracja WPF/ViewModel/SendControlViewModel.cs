using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using App.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GPS.EDeklaracje;
using Microsoft.Practices.ServiceLocation;

namespace E_Deklaracja_WPF.ViewModel
{
    public class SendControlViewModel : ViewModelBase
    {
        private bool _progressBarVisible = false;
        public bool ProgressBarVisible
        {
            get { return _progressBarVisible; }
            set
            {
                _progressBarVisible = value;

                RaisePropertyChanged("ProgressBarVisible");
            }
        }

        private string _sendButtonCaption = "Wyślij e-deklarację...";
        public string SendButtonCaption
        {
            get { return _sendButtonCaption; }
            set
            {
                _sendButtonCaption = value;

                RaisePropertyChanged("SendButtonCaption");
            }
        }
        

        private bool _sendButtonEnabled = true;
        public bool SendButtonEnabled
        {
            get { return _sendButtonEnabled; }
            set
            {
                _sendButtonEnabled = value;

                RaisePropertyChanged("SendButtonEnabled");
            }
        }

        TypKomunikacjiMF typ = TypKomunikacjiMF.Oficjalny;
        public TypKomunikacjiMF TypKomunikacjiMF
        {
            get { return typ; }
            set
            {
                typ = value;

                RaisePropertyChanged("TypKomunikacjiMF");
            }
        }

        private string _edekPath = "";
        public string EdekPath
        {
            get { return _edekPath; }
            set
            {
                _edekPath = value;

                RaisePropertyChanged("EdekPath");
            }
        }

        IList<System.Security.Cryptography.X509Certificates.X509Certificate2> certificates = new List<System.Security.Cryptography.X509Certificates.X509Certificate2>();
        public IList<System.Security.Cryptography.X509Certificates.X509Certificate2> Certificates
        {
            get
            {
                return certificates;
            }
            set
            {
                certificates = value;
                
                RaisePropertyChanged("Certificates");
            }
        }

        public IOpenDialogService OpenDialogService { get { return ServiceLocator.Current.GetInstance<IOpenDialogService>(); } }

        private ICommand _openFileCommand;
        public ICommand OpenFileCommand
        {
            get { return this._openFileCommand ?? (this._openFileCommand = new RelayCommand(OnOpenFileCommand)); }
        }

        private ICommand _sendEdekCommand;
        public ICommand SendEdekCommand
        {
            get { return this._sendEdekCommand ?? (this._sendEdekCommand = new RelayCommand(OnSendEdekCommand)); }
        }

        public SendControlViewModel()
        {
            this.LoadCertificates();
        }

        private void OnOpenFileCommand()
        {
            string path = OpenDialogService.OpenFileDialog(string.Empty);

            if (!string.IsNullOrEmpty(path))
                this.EdekPath = path;
        }

        private void OnSendEdekCommand()
        {
            this.SendButtonCaption = "Trwa wysyłanie e-deklaracji...";
            this.ProgressBarVisible = true;
            this.SendButtonEnabled = false;            

            DoAsyncWork();            
        }

        private async void DoAsyncWork()
        {
            await Task.Run(() =>
            {
                int size = 0;
                for (int z = 0; z < 100; z++)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        string value = i.ToString();
                        if (value == null)
                        {
                            size = 0;
                        }
                        size += value.Length;
                    }
                }

                this.ProgressBarVisible = false;
                this.SendButtonEnabled = true;
                this.SendButtonCaption = "Wyślij e-deklarację...";
            });
        }

        private void LoadCertificates()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            store.Open(OpenFlags.OpenExistingOnly);

            List<X509Certificate2> certificates = new List<X509Certificate2>();
            foreach (var item in store.Certificates)
            {
                certificates.Add(item);
            }

            this.Certificates = certificates;
        }  
    }
}

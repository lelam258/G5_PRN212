using Business_Layer;
using Repositories.Interfaces;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using CommunityToolkit.Mvvm.Input;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for StudentCertificatePage.xaml
    /// </summary>
    public partial class StudentCertificatePage : Page, INotifyPropertyChanged
    {
        // 1) Collection bind lên DataGrid
        public ObservableCollection<Certificate> Certificates { get; } = new();

        private readonly int _studentId;
        private readonly ICertificateRepository _certRepo;

        // 2) Download command
        public ICommand DownloadCommand { get; }

        private string _lastUpdatedText = "";
        public string LastUpdatedText
        {
            get => _lastUpdatedText;
            set { _lastUpdatedText = value; OnPropertyChanged(); }
        }

        public StudentCertificatePage()
        {
            InitializeComponent();
            DataContext = this;
        }
        public StudentCertificatePage(int studentId)
          : this()
        {
            _studentId = studentId;
            _certRepo = new CertificateRepository();
            DownloadCommand = new RelayCommand<string>(DownloadFile);
            LoadCertificates();
        }

        private void LoadCertificates()
        {
            Certificates.Clear();
            var list = _certRepo.GetCertificatesByStudentId(_studentId);
            foreach (var cert in list)
                Certificates.Add(cert);

            LastUpdatedText = DateTime.Now.ToString("HH:mm tt +07, dd MMMM yyyy");
        }

        private void DownloadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Không có đường dẫn file.");
                return;
            }
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không mở được file: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}

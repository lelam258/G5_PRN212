using Business_Layer;
using Repositories.Interfaces;
using Repositories.Repositories;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Presentation_Layer
{
    public partial class StudentCertificatePage : Page
    {
        private readonly ICertificateRepository _certificateRepository;
        public ObservableCollection<Certificate> Certificates { get; set; }
        public RelayCommand<string> DownloadCommand { get; set; } // Adjusted to use your custom RelayCommand

        public StudentCertificatePage()
        {
            InitializeComponent();
            _certificateRepository = new CertificateRepository();
            Certificates = new ObservableCollection<Certificate>();
            DownloadCommand = new RelayCommand<string>(DownloadCertificate); // Assuming your RelayCommand accepts a delegate
            DataContext = this;
            LoadCertificates();
        }

        private void LoadCertificates()
        {
            // Assuming StudentId is available, e.g., from a logged-in user context
            int studentId = 1; // Replace with actual student ID from authentication context
            var certificates = _certificateRepository.GetCertificatesByStudentId(studentId);
            Certificates.Clear();
            foreach (var certificate in certificates)
            {
                Certificates.Add(certificate);
            }
        }

        private void DownloadCertificate(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    MessageBox.Show("Certificate file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Open the file using the default application
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading certificate: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
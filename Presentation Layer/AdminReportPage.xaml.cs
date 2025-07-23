using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Text;
using Microsoft.Win32;
using Business_Layer;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Threading.Tasks;

namespace Presentation_Layer
{
    public partial class AdminReportPage : Page
    {
        private readonly StudentRepository _studentRepo;
        private readonly AssessmentResultRepository _resultRepo;
        private readonly EnrollmentRepository _enrollRepo;
        private readonly CertificateRepository _certRepo;

        public class ReportItem
        {
            public string Metric { get; set; }
            public double Value { get; set; }
            public string DisplayValue => Value.ToString("N2");
            public string Description { get; set; }
        }

        public AdminReportPage()
        {
            InitializeComponent();

            // Initialize repositories with error handling
            try
            {
                _studentRepo = new StudentRepository();
                _resultRepo = new AssessmentResultRepository();
                _enrollRepo = new EnrollmentRepository();
                _certRepo = new CertificateRepository();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo repositories: {ex.Message}", "Lỗi khởi tạo",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Loaded += Page_Loaded;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show loading indicator
                LoadingGrid.Visibility = Visibility.Visible;
                ContentGrid.Visibility = Visibility.Collapsed;

                await Task.Delay(100); // Allow UI to update

                // Test repositories and load report
                await TestAndLoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải trang: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingGrid.Visibility = Visibility.Collapsed;
                ContentGrid.Visibility = Visibility.Visible;
            }
        }

        private async Task TestAndLoadDataAsync()
        {
            var testResults = new List<string>();
            var reportItems = new List<ReportItem>();

            try
            {
                // Test Student Repository
                await TestStudentRepository(testResults, reportItems);

                // Test Assessment Repository  
                await TestAssessmentRepository(testResults, reportItems);

                // Test Enrollment Repository
                await TestEnrollmentRepository(testResults, reportItems);

                // Test Certificate Repository
                await TestCertificateRepository(testResults, reportItems);

                // Log all test results
                LogTestResults(testResults);

                // Ensure we have some data to display
                if (!reportItems.Any())
                {
                    reportItems.Add(new ReportItem
                    {
                        Metric = "Trạng thái hệ thống",
                        Value = 0,
                        Description = "Không có dữ liệu khả dụng"
                    });
                }

                // Update UI
                ReportDataGrid.ItemsSource = reportItems;
                ChartItemsControl.ItemsSource = reportItems;

                // Update status
                StatusTextBlock.Text = $"Đã tải {reportItems.Count} chỉ số thống kê - Cập nhật lúc {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}",
                               "Lỗi tải dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Show error state
                var errorItems = new List<ReportItem>
                {
                    new ReportItem { Metric = "Lỗi tải dữ liệu", Value = 0, Description = ex.Message }
                };
                ReportDataGrid.ItemsSource = errorItems;
                ChartItemsControl.ItemsSource = errorItems;
                StatusTextBlock.Text = "Có lỗi xảy ra khi tải dữ liệu";
            }
        }

        private async Task TestStudentRepository(List<string> testResults, List<ReportItem> reportItems)
        {
            try
            {
                if (_studentRepo == null)
                {
                    testResults.Add("StudentRepository: NULL - chưa được khởi tạo");
                    return;
                }

                var students = await Task.Run(() => _studentRepo.GetAllStudents());
                var studentCount = students?.Count() ?? 0;
                testResults.Add($"Students: {studentCount} records");

                reportItems.Add(new ReportItem
                {
                    Metric = "Tổng số sinh viên",
                    Value = studentCount,
                    Description = $"{studentCount} sinh viên trong hệ thống"
                });

                if (students?.Any() == true)
                {
                    // Group by status
                    var statusGroups = students
                        .Where(s => !string.IsNullOrEmpty(s.Status))
                        .GroupBy(s => s.Status)
                        .ToList();

                    foreach (var group in statusGroups)
                    {
                        reportItems.Add(new ReportItem
                        {
                            Metric = $"SV {group.Key}",
                            Value = group.Count(),
                            Description = $"Sinh viên có trạng thái {group.Key}"
                        });
                        testResults.Add($"  - Status '{group.Key}': {group.Count()} students");
                    }

                    // Sample student info
                    var firstStudent = students.First();
                    testResults.Add($"Sample student: {firstStudent.StudentName} (ID: {firstStudent.StudentId})");
                }
            }
            catch (Exception ex)
            {
                testResults.Add($"Students ERROR: {ex.Message}");
                reportItems.Add(new ReportItem
                {
                    Metric = "Lỗi tải SV",
                    Value = 0,
                    Description = ex.Message
                });
            }
        }

        private async Task TestAssessmentRepository(List<string> testResults, List<ReportItem> reportItems)
        {
            try
            {
                if (_resultRepo == null)
                {
                    testResults.Add("AssessmentResultRepository: NULL");
                    return;
                }

                var results = await Task.Run(() => _resultRepo.GetAllAssessmentResults());
                var resultCount = results?.Count() ?? 0;
                testResults.Add($"AssessmentResults: {resultCount} records");

                reportItems.Add(new ReportItem
                {
                    Metric = "Số bài kiểm tra",
                    Value = resultCount,
                    Description = $"{resultCount} kết quả đánh giá"
                });

                if (results?.Any() == true)
                {
                    var validResults = results.Where(r => r.Score > 0).ToList();
                    if (validResults.Any())
                    {
                        var avgScore = validResults.Average(r => (double)r.Score);
                        reportItems.Add(new ReportItem
                        {
                            Metric = "Điểm trung bình",
                            Value = avgScore,
                            Description = $"Điểm TB từ {validResults.Count} bài thi hợp lệ"
                        });
                        testResults.Add($"Average score: {avgScore:F2} from {validResults.Count} valid results");
                    }
                }
                else
                {
                    reportItems.Add(new ReportItem
                    {
                        Metric = "Điểm trung bình",
                        Value = 0,
                        Description = "Chưa có dữ liệu điểm"
                    });
                }
            }
            catch (Exception ex)
            {
                testResults.Add($"AssessmentResults ERROR: {ex.Message}");
                if (ex.Message.Contains("SubmissionFilePath"))
                {
                    testResults.Add("  -> Lỗi cột SubmissionFilePath không tồn tại trong database");
                    testResults.Add("  -> Cần kiểm tra schema của bảng AssessmentResults");
                }

                reportItems.Add(new ReportItem
                {
                    Metric = "Lỗi đánh giá",
                    Value = 0,
                    Description = "Lỗi truy vấn bảng đánh giá"
                });
            }
        }

        private async Task TestEnrollmentRepository(List<string> testResults, List<ReportItem> reportItems)
        {
            try
            {
                if (_enrollRepo == null)
                {
                    testResults.Add("EnrollmentRepository: NULL");
                    return;
                }

                var enrollments = await Task.Run(() => _enrollRepo.GetAllEnrollments());
                var enrollCount = enrollments?.Count() ?? 0;
                testResults.Add($"Enrollments: {enrollCount} records");

                reportItems.Add(new ReportItem
                {
                    Metric = "Số lượt đăng ký",
                    Value = enrollCount,
                    Description = $"{enrollCount} lượt đăng ký khóa học"
                });

                if (enrollments?.Any() == true)
                {
                    var completedCount = enrollments.Count(e => e.CompletionStatus == true);
                    var completionRate = enrollCount > 0 ? (100.0 * completedCount / enrollCount) : 0;

                    reportItems.Add(new ReportItem
                    {
                        Metric = "Tỷ lệ hoàn thành (%)",
                        Value = completionRate,
                        Description = $"{completedCount}/{enrollCount} hoàn thành"
                    });

                    testResults.Add($"Completion rate: {completionRate:F1}% ({completedCount}/{enrollCount})");
                }
                else
                {
                    reportItems.Add(new ReportItem
                    {
                        Metric = "Tỷ lệ hoàn thành (%)",
                        Value = 0,
                        Description = "Chưa có dữ liệu đăng ký"
                    });
                }
            }
            catch (Exception ex)
            {
                testResults.Add($"Enrollments ERROR: {ex.Message}");
                reportItems.Add(new ReportItem
                {
                    Metric = "Lỗi đăng ký",
                    Value = 0,
                    Description = "Lỗi truy vấn bảng đăng ký"
                });
            }
        }

        private async Task TestCertificateRepository(List<string> testResults, List<ReportItem> reportItems)
        {
            try
            {
                if (_certRepo == null)
                {
                    testResults.Add("CertificateRepository: NULL");
                    return;
                }

                var certificates = await Task.Run(() => _certRepo.GetAllCertificates());
                var certCount = certificates?.Count() ?? 0;
                testResults.Add($"Certificates: {certCount} records");

                reportItems.Add(new ReportItem
                {
                    Metric = "Chứng chỉ đã cấp",
                    Value = certCount,
                    Description = $"{certCount} chứng chỉ đã được cấp"
                });
            }
            catch (Exception ex)
            {
                testResults.Add($"Certificates ERROR: {ex.Message}");
                reportItems.Add(new ReportItem
                {
                    Metric = "Lỗi chứng chỉ",
                    Value = 0,
                    Description = "Lỗi truy vấn bảng chứng chỉ"
                });
            }
        }

        private void LogTestResults(List<string> testResults)
        {
            var logMessage = string.Join("\n", testResults);
            System.Diagnostics.Debug.WriteLine($"\n=== REPOSITORY TEST RESULTS ===\n{logMessage}\n===============================");

            // Optionally write to file for debugging
            try
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                         $"AdminReport_Debug_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                File.WriteAllText(logPath, $"Admin Report Debug Log - {DateTime.Now}\n\n{logMessage}");
            }
            catch
            {
                // Ignore file write errors
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RefreshButton.IsEnabled = false;
                RefreshButton.Content = "🔄 Đang tải...";

                await TestAndLoadDataAsync();

                MessageBox.Show("Dữ liệu đã được làm mới thành công!", "Thành công",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                RefreshButton.IsEnabled = true;
                RefreshButton.Content = "🔄 Làm mới";
            }
        }

        private void ExportExcelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var items = ReportDataGrid.ItemsSource as List<ReportItem>;
                if (items == null || !items.Any())
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Lưu báo cáo Excel",
                    Filter = "Excel Files (*.csv)|*.csv|All Files (*.*)|*.*",
                    DefaultExt = "csv",
                    FileName = $"BaoCaoThongKe_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToCSV(saveDialog.FileName, items);
                    MessageBox.Show($"Xuất báo cáo Excel thành công!\nĐường dẫn: {saveDialog.FileName}",
                                   "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var items = ReportDataGrid.ItemsSource as List<ReportItem>;
                if (items == null || !items.Any())
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Lưu báo cáo PDF",
                    Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                    DefaultExt = "pdf",
                    FileName = $"BaoCaoThongKe_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToPDF(saveDialog.FileName, items);
                    MessageBox.Show($"Xuất báo cáo PDF thành công!\nĐường dẫn: {saveDialog.FileName}",
                                   "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToCSV(string filePath, List<ReportItem> items)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write BOM for UTF-8
                writer.Write('\ufeff');

                // Write header
                writer.WriteLine("Chỉ số,Giá trị,Mô tả");

                // Write data
                foreach (var item in items)
                {
                    writer.WriteLine($"\"{item.Metric}\",{item.Value:N2},\"{item.Description ?? ""}\"");
                }
            }
        }

        private void ExportToPDF(string filePath, List<ReportItem> items)
        {
            // Create PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Báo cáo thống kê hệ thống";
            document.Info.Author = "Hệ thống quản lý";
            document.Info.CreationDate = DateTime.Now;

            // Create new page
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;

            // Create graphics context
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Define fonts with fallback
            XFont titleFont, headerFont, normalFont, smallFont;
            try
            {
                titleFont = new XFont("Times New Roman", 20);
                headerFont = new XFont("Times New Roman", 14);
                normalFont = new XFont("Times New Roman", 12);
                smallFont = new XFont("Times New Roman", 10);
            }
            catch
            {
                titleFont = new XFont("Arial", 20);
                headerFont = new XFont("Arial", 14);
                normalFont = new XFont("Arial", 12);
                smallFont = new XFont("Arial", 10);
            }

            // Define colors and brushes
            XBrush blackBrush = XBrushes.Black;
            XBrush blueBrush = new XSolidBrush(XColor.FromArgb(44, 62, 80));
            XBrush grayBrush = XBrushes.LightGray;
            XPen blackPen = XPens.Black;

            // Layout settings
            double yPosition = 60;
            double leftMargin = 50;
            double rightMargin = page.Width - 50;
            double pageWidth = rightMargin - leftMargin;

            // Title
            gfx.DrawString("BÁO CÁO THỐNG KÊ HỆ THỐNG", titleFont, blueBrush,
                          new XRect(leftMargin, yPosition, pageWidth, 30), XStringFormats.TopCenter);
            yPosition += 50;

            // Date
            string dateText = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            gfx.DrawString(dateText, normalFont, blackBrush,
                          new XRect(leftMargin, yPosition, pageWidth, 20), XStringFormats.TopRight);
            yPosition += 40;

            // Table
            double tableWidth = pageWidth;
            double col1Width = tableWidth * 0.4;
            double col2Width = tableWidth * 0.2;
            double col3Width = tableWidth * 0.4;
            double rowHeight = 25;

            // Table header
            XRect[] headerRects = {
                new XRect(leftMargin, yPosition, col1Width, rowHeight),
                new XRect(leftMargin + col1Width, yPosition, col2Width, rowHeight),
                new XRect(leftMargin + col1Width + col2Width, yPosition, col3Width, rowHeight)
            };

            foreach (var rect in headerRects)
            {
                gfx.DrawRectangle(blueBrush, rect);
                gfx.DrawRectangle(blackPen, rect);
            }

            gfx.DrawString("Chỉ số", headerFont, XBrushes.White,
                          new XRect(leftMargin + 5, yPosition + 5, col1Width - 10, rowHeight - 10),
                          XStringFormats.CenterLeft);
            gfx.DrawString("Giá trị", headerFont, XBrushes.White,
                          new XRect(leftMargin + col1Width + 5, yPosition + 5, col2Width - 10, rowHeight - 10),
                          XStringFormats.Center);
            gfx.DrawString("Mô tả", headerFont, XBrushes.White,
                          new XRect(leftMargin + col1Width + col2Width + 5, yPosition + 5, col3Width - 10, rowHeight - 10),
                          XStringFormats.CenterLeft);

            yPosition += rowHeight;

            // Table data
            bool alternateRow = false;
            foreach (var item in items)
            {
                if (yPosition > page.Height - 100)
                {
                    gfx.Dispose();
                    page = document.AddPage();
                    page.Size = PdfSharp.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    yPosition = 50;
                }

                XRect[] dataRects = {
                    new XRect(leftMargin, yPosition, col1Width, rowHeight),
                    new XRect(leftMargin + col1Width, yPosition, col2Width, rowHeight),
                    new XRect(leftMargin + col1Width + col2Width, yPosition, col3Width, rowHeight)
                };

                if (alternateRow)
                {
                    foreach (var rect in dataRects)
                        gfx.DrawRectangle(grayBrush, rect);
                }

                foreach (var rect in dataRects)
                    gfx.DrawRectangle(blackPen, rect);

                gfx.DrawString(item.Metric ?? "", normalFont, blackBrush,
                              new XRect(leftMargin + 5, yPosition + 5, col1Width - 10, rowHeight - 10),
                              XStringFormats.CenterLeft);
                gfx.DrawString(item.Value.ToString("N2"), normalFont, blackBrush,
                              new XRect(leftMargin + col1Width + 5, yPosition + 5, col2Width - 10, rowHeight - 10),
                              XStringFormats.Center);
                gfx.DrawString(item.Description ?? "", smallFont, blackBrush,
                              new XRect(leftMargin + col1Width + col2Width + 5, yPosition + 5, col3Width - 10, rowHeight - 10),
                              XStringFormats.CenterLeft);

                yPosition += rowHeight;
                alternateRow = !alternateRow;
            }

            // Footer
            yPosition = page.Height - 50;
            gfx.DrawString("Báo cáo được tạo bởi hệ thống quản lý", smallFont, blackBrush,
                          new XRect(leftMargin, yPosition, pageWidth, 20), XStringFormats.TopCenter);

            gfx.Dispose();
            document.Save(filePath);
            document.Close();
        }
    }

    
}
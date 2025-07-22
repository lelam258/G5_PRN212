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

namespace Presentation_Layer
{
    public partial class AdminReportPage : Page
    {
        private readonly StudentRepository _studentRepo = new StudentRepository();
        private readonly AssessmentResultRepository _resultRepo = new AssessmentResultRepository();
        private readonly EnrollmentRepository _enrollRepo = new EnrollmentRepository();
        private readonly CertificateRepository _certRepo = new CertificateRepository();

        public class ReportItem
        {
            public string Metric { get; set; }
            public double Value { get; set; }
        }

        public AdminReportPage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            var items = new List<ReportItem>();

            try
            {
                // Students by status
                var students = _studentRepo.GetAllStudents();
                if (students?.Any() == true)
                {
                    var byStatus = students
                                  .GroupBy(s => s.Status)
                                  .Select(g => new ReportItem { Metric = $"Sinh viên {g.Key}", Value = g.Count() });
                    items.AddRange(byStatus);
                }

                // Average score
                var results = _resultRepo.GetAllAssessmentResults();
                if (results?.Any() == true)
                {
                    var avgScore = results.Average(r => (double)r.Score);
                    items.Add(new ReportItem { Metric = "Điểm trung bình", Value = avgScore });
                }

                // Completion rate (%)
                var enrolls = _enrollRepo.GetAllEnrollments();
                if (enrolls?.Any() == true)
                {
                    var completed = enrolls.Count(e => e.CompletionStatus);
                    var rate = 100.0 * completed / enrolls.Count();
                    items.Add(new ReportItem { Metric = "Tỷ lệ hoàn thành (%)", Value = rate });
                }

                // Certificates issued
                var certCount = _certRepo.GetAllCertificates()?.Count ?? 0;
                items.Add(new ReportItem { Metric = "Chứng chỉ đã cấp", Value = certCount });

                // Set data to both controls
                ReportDataGrid.ItemsSource = items;
                ChartItemsControl.ItemsSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportExcelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tạo SaveFileDialog cho Excel
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Lưu báo cáo Excel",
                    Filter = "Excel Files (*.csv)|*.csv|All Files (*.*)|*.*",
                    DefaultExt = "csv",
                    FileName = $"BaoCaoThongKe_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToCSV(saveDialog.FileName);
                    MessageBox.Show($"Xuất báo cáo Excel thành công!\nĐường dẫn: {saveDialog.FileName}",
                                   "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tạo SaveFileDialog cho PDF
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Lưu báo cáo PDF",
                    Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                    DefaultExt = "pdf",
                    FileName = $"BaoCaoThongKe_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToPDF(saveDialog.FileName);
                    MessageBox.Show($"Xuất báo cáo PDF thành công!\nĐường dẫn: {saveDialog.FileName}",
                                   "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToCSV(string filePath)
        {
            var items = ReportDataGrid.ItemsSource as List<ReportItem>;
            if (items == null || !items.Any()) return;

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Viết header
                writer.WriteLine("Chỉ số,Giá trị");

                // Viết dữ liệu
                foreach (var item in items)
                {
                    writer.WriteLine($"\"{item.Metric}\",{item.Value:N2}");
                }
            }
        }

        private void ExportToPDF(string filePath)
        {
            var items = ReportDataGrid.ItemsSource as List<ReportItem>;
            if (items == null || !items.Any()) return;

            // Tạo PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Báo cáo thống kê";
            document.Info.Author = "Hệ thống quản lý";
            document.Info.CreationDate = DateTime.Now;

            // Tạo trang mới
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;

            // Tạo graphics context
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Định nghĩa fonts
            XFont titleFont = new XFont("Times New Roman", 20);
            XFont headerFont = new XFont("Times New Roman", 14);
            XFont normalFont = new XFont("Times New Roman", 12);
            XFont smallFont = new XFont("Times New Roman", 10);


            // Định nghĩa colors
            XBrush blackBrush = XBrushes.Black;
            XBrush blueBrush = new XSolidBrush(XColor.FromArgb(44, 62, 80)); // #2C3E50
            XBrush grayBrush = XBrushes.LightGray;
            XPen blackPen = XPens.Black;

            // Vị trí bắt đầu
            double yPosition = 60;
            double leftMargin = 50;
            double rightMargin = page.Width - 50;
            double pageWidth = rightMargin - leftMargin;

            // Tiêu đề
            gfx.DrawString("BÁO CÁO THỐNG KÊ", titleFont, blueBrush,
                          new XRect(leftMargin, yPosition, pageWidth, 30), XStringFormats.TopCenter);
            yPosition += 50;

            // Ngày tạo
            string dateText = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            gfx.DrawString(dateText, normalFont, blackBrush,
                          new XRect(leftMargin, yPosition, pageWidth, 20), XStringFormats.TopRight);
            yPosition += 40;

            // Vẽ bảng
            double tableWidth = pageWidth;
            double col1Width = tableWidth * 0.7; 
            double col2Width = tableWidth * 0.3; 
            double rowHeight = 25;

            // Header của bảng
            XRect headerRect1 = new XRect(leftMargin, yPosition, col1Width, rowHeight);
            XRect headerRect2 = new XRect(leftMargin + col1Width, yPosition, col2Width, rowHeight);

            gfx.DrawRectangle(blueBrush, headerRect1);
            gfx.DrawRectangle(blueBrush, headerRect2);
            gfx.DrawRectangle(blackPen, headerRect1);
            gfx.DrawRectangle(blackPen, headerRect2);

            gfx.DrawString("Chỉ số", headerFont, XBrushes.White,
                          new XRect(leftMargin + 5, yPosition + 5, col1Width - 10, rowHeight - 10),
                          XStringFormats.CenterLeft);
            gfx.DrawString("Giá trị", headerFont, XBrushes.White,
                          new XRect(leftMargin + col1Width + 5, yPosition + 5, col2Width - 10, rowHeight - 10),
                          XStringFormats.Center);

            yPosition += rowHeight;

            // Dữ liệu bảng
            bool alternateRow = false;
            foreach (var item in items)
            {
                // Kiểm tra nếu cần trang mới
                if (yPosition > page.Height - 100)
                {
                    gfx.Dispose();
                    page = document.AddPage();
                    page.Size = PdfSharp.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    yPosition = 50;
                }

                XRect dataRect1 = new XRect(leftMargin, yPosition, col1Width, rowHeight);
                XRect dataRect2 = new XRect(leftMargin + col1Width, yPosition, col2Width, rowHeight);

                // Màu nền xen kẽ
                if (alternateRow)
                {
                    gfx.DrawRectangle(grayBrush, dataRect1);
                    gfx.DrawRectangle(grayBrush, dataRect2);
                }

                // Viền
                gfx.DrawRectangle(blackPen, dataRect1);
                gfx.DrawRectangle(blackPen, dataRect2);

                // Text
                gfx.DrawString(item.Metric, normalFont, blackBrush,
                              new XRect(leftMargin + 5, yPosition + 5, col1Width - 10, rowHeight - 10),
                              XStringFormats.CenterLeft);
                gfx.DrawString(item.Value.ToString("N2"), normalFont, blackBrush,
                              new XRect(leftMargin + col1Width + 5, yPosition + 5, col2Width - 10, rowHeight - 10),
                              XStringFormats.Center);

                yPosition += rowHeight;
                alternateRow = !alternateRow;
            }

            // Footer
            yPosition = page.Height - 50;
            gfx.DrawString("Báo cáo được tạo bởi hệ thống quản lý", smallFont, blackBrush,
                          new XRect(leftMargin, yPosition, pageWidth, 20), XStringFormats.TopCenter);

            // Giải phóng resources
            gfx.Dispose();

            // Lưu file
            document.Save(filePath);
            document.Close();
        }

        private string GenerateHTMLReport(List<ReportItem> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='utf-8'>");
            sb.AppendLine("<title>Báo cáo thống kê</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; margin: 40px; }");
            sb.AppendLine("h1 { color: #2C3E50; text-align: center; }");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
            sb.AppendLine("th, td { border: 1px solid #ddd; padding: 12px; text-align: left; }");
            sb.AppendLine("th { background-color: #2C3E50; color: white; }");
            sb.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
            sb.AppendLine(".footer { margin-top: 30px; text-align: center; font-size: 12px; color: #666; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine("<h1>Báo cáo thống kê</h1>");
            sb.AppendLine($"<p><strong>Ngày xuất:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");

            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Chỉ số</th><th>Giá trị</th></tr>");

            foreach (var item in items)
            {
                sb.AppendLine($"<tr><td>{item.Metric}</td><td>{item.Value:N2}</td></tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("<div class='footer'>Báo cáo được tạo bởi hệ thống quản lý</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }

    // Value to Width Converter for simple bar chart visualization
    public class ValueToWidthConverter : IValueConverter
    {
        public static ValueToWidthConverter Instance = new ValueToWidthConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                // Scale the value to fit in a reasonable width (max 200 pixels)
                // You can adjust the scaling factor based on your data range
                double scaledValue = Math.Min(doubleValue * 2, 200);
                return Math.Max(scaledValue, 5); // Minimum width of 5 pixels
            }
            return 5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using Business_Layer;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Presentation_Layer
{
    public partial class AdminPaymentPage : Page
    {
        private readonly PaymentRepository _paymentRepo = new();
        private readonly StudentRepository _studentRepo = new();
        private readonly LifeSkillCourseRepository _courseRepo = new();
        public AdminPaymentPage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Populate filter status
            // (ComboBox items defined in XAML)

            // Populate form student & course
            StudentFormComboBox.ItemsSource = _studentRepo.GetAllStudents();
            CourseFormComboBox.ItemsSource = _courseRepo.GetAllLifeSkillCourses();

            FilterPayments();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
            => FilterPayments();

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            FilterStatusComboBox.SelectedIndex = 0; // "Tất cả"
            FilterPayments();
        }

        private void FilterPayments()
        {
            var list = _paymentRepo.GetAllPayments().AsQueryable();
            var status = (FilterStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (status == "Đã thanh toán")
                list = list.Where(p => p.Status == "Đã thanh toán");
            else if (status == "Chưa thanh toán")
                list = list.Where(p => p.Status == "Chưa thanh toán");

            PaymentDataGrid.ItemsSource = list.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePayment(out var payment)) return;
            _paymentRepo.AddPayment(payment);
            FilterPayments(); ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentDataGrid.SelectedItem is not Payment sel) return;
            if (!ValidatePayment(out var updated)) return;
            updated.PaymentId = sel.PaymentId;
            _paymentRepo.UpdatePayment(updated);
            FilterPayments();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentDataGrid.SelectedItem is not Payment sel) return;
            _paymentRepo.DeletePayment(sel.PaymentId);
            FilterPayments(); ClearForm();
        }

        private void PaymentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentDataGrid.SelectedItem is Payment sel)
            {
                StudentFormComboBox.SelectedItem = _studentRepo.GetAllStudents().First(s => s.StudentId == sel.StudentId);
                CourseFormComboBox.SelectedItem = _courseRepo.GetAllLifeSkillCourses().First(c => c.CourseId == sel.CourseId);
                AmountTextBox.Text = sel.Amount.ToString("F0");
                PaymentDatePicker.SelectedDate = sel.PaymentDate;
                StatusFormComboBox.SelectedItem = (sel.Status == "Đã thanh toán")
                    ? StatusFormComboBox.Items[0]
                    : StatusFormComboBox.Items[1];
                UpdateButton.IsEnabled = DeleteButton.IsEnabled = true;
            }
            else ClearForm();
        }

        private bool ValidatePayment(out Payment payment)
        {
            payment = null;
            var stu = StudentFormComboBox.SelectedItem as Student;
            var cou = CourseFormComboBox.SelectedItem as LifeSkillCourse;
            if (stu is null || cou is null)
            {
                MessageBox.Show("Chọn sinh viên và khóa học."); return false;
            }
            if (!decimal.TryParse(AmountTextBox.Text, out var amt) || amt <= 0)
            {
                MessageBox.Show("Số tiền không hợp lệ."); return false;
            }
            if (PaymentDatePicker.SelectedDate is not DateTime pd)
            {
                MessageBox.Show("Chọn ngày thanh toán."); return false;
            }
            if (pd.Date < DateTime.Today)
            {
                MessageBox.Show("Ngày thanh toán không được trước hôm nay."); return false;
            }
            var status = (StatusFormComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (status != "Đã thanh toán" && status != "Chưa thanh toán")
            {
                MessageBox.Show("Chọn trạng thái."); return false;
            }
            payment = new Payment
            {
                StudentId = stu.StudentId,
                CourseId = cou.CourseId,
                Amount = amt,
                PaymentDate = pd,
                Status = status
            };
            return true;
        }

        private void ClearForm()
        {
            StudentFormComboBox.SelectedItem = null;
            CourseFormComboBox.SelectedItem = null;
            AmountTextBox.Clear();
            PaymentDatePicker.SelectedDate = null;
            StatusFormComboBox.SelectedIndex = -1;
            UpdateButton.IsEnabled = DeleteButton.IsEnabled = false;
        }
    }
}
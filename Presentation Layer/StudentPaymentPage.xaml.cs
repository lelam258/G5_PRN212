using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Business_Layer;
using Repositories.Repositories;

namespace Presentation_Layer
{
    public partial class StudentPaymentPage : Page
    {
        public ObservableCollection<PaymentDisplayItem> PaymentItems { get; set; } = new();

        private readonly int _studentId;
        private readonly PaymentRepository _paymentRepo = new();
        private readonly LifeSkillCourseRepository _courseRepo = new();

        public StudentPaymentPage(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;
            DataContext = this;
            LoadPayments();
        }

        private void LoadPayments()
        {
            var payments = _paymentRepo.GetAllPayments()
                                       .Where(p => p.StudentId == _studentId)
                                       .ToList();

            foreach (var payment in payments)
            {
                var course = _courseRepo.GetLifeSkillCourseById(payment.CourseId);
                PaymentItems.Add(new PaymentDisplayItem
                {
                    CourseName = course?.CourseName ?? "(Không rõ)",
                    Amount = $"{payment.Amount:#,##0} VND",
                    PaymentDate = payment.PaymentDate.ToString("dd/MM/yyyy"),
                    Status = payment.Status
                });
            }
        }

        public class PaymentDisplayItem
        {
            public string CourseName { get; set; }
            public string Amount { get; set; }
            public string PaymentDate { get; set; }
            public string Status { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;
using Data_Layer;
using Repositories.Interfaces;

namespace Repositories.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDAO _paymentDAO;
        public PaymentRepository()
        {
            _paymentDAO = new PaymentDAO();
        }
        public void AddPayment(Payment payment) => _paymentDAO.AddPayment(payment);
        public void DeletePayment(int id) => _paymentDAO.DeletePayment(id);
        public Payment GetPaymentById(int id) => _paymentDAO.GetPaymentById(id);
        public List<Payment> GetAllPayments() => _paymentDAO.GetAllPayments();
        public void UpdatePayment(Payment payment) => _paymentDAO.UpdatePayment(payment);
    }
}

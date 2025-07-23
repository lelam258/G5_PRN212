using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class PaymentDAO
    {
        private readonly ApplicationDbContext _context;
        public PaymentDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<Payment> GetAllPayments()
        {
            return _context.Payments.ToList();
        }
        public Payment GetPaymentById(int id)
        {
            return _context.Payments.FirstOrDefault(p => p.PaymentId == id);
        }
        public void AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            _context.SaveChanges();
        }
        public void UpdatePayment(Payment payment)
        {
            _context.Payments.Update(payment);
            _context.SaveChanges();
        }
        public void DeletePayment(int id)
        {
            var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                _context.SaveChanges();
            }
        }
    }
}

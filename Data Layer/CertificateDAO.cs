using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class CertificateDAO
    {
        private readonly ApplicationDbContext _context;
        public CertificateDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<Certificate> GetAllCertificates()
        {
            return _context.Certificates.ToList();
        }
        public Certificate GetCertificateById(int id)
        {
            return _context.Certificates.FirstOrDefault(c => c.CertificateId == id);
        }
        public void AddCertificate(Certificate certificate)
        {
            _context.Certificates.Add(certificate);
            _context.SaveChanges();
        }
        public void UpdateCertificate(Certificate certificate)
        {
            _context.Certificates.Update(certificate);
            _context.SaveChanges();
        }
        public void DeleteCertificate(int id)
        {
            var certificate = _context.Certificates.FirstOrDefault(c => c.CertificateId == id);
            if (certificate != null)
            {
                _context.Certificates.Remove(certificate);
                _context.SaveChanges();
            }
        }
    }
}

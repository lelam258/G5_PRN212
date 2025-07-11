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
    public class CertificateRepository : ICertificateRepository
    {
        private readonly CertificateDAO _certificateDAO;
        public CertificateRepository()
        {
            _certificateDAO = new CertificateDAO();
        }
        public void AddCertificate(Certificate certificate) => _certificateDAO.AddCertificate(certificate);
        public void DeleteCertificate(int id) => _certificateDAO.DeleteCertificate(id);
        public Certificate GetCertificateById(int id) => _certificateDAO.GetCertificateById(id);
        public List<Certificate> GetAllCertificates() => _certificateDAO.GetAllCertificates();
        public void UpdateCertificate(Certificate certificate) => _certificateDAO.UpdateCertificate(certificate);
    }
}

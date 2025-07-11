using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface ICertificateRepository
    {
        List<Certificate> GetAllCertificates();
        Certificate GetCertificateById(int id);
        void AddCertificate(Certificate certificate);
        void UpdateCertificate(Certificate certificate);
        void DeleteCertificate(int id);
    }
}

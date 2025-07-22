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
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly EnrollmentDAO _enrollmentDAO;
         
    public List<Enrollment> GetEnrollmentsByStudentId(int studentId)
        => _enrollmentDAO.GetEnrollmentsByStudentId(studentId);
        public EnrollmentRepository()
        {
            _enrollmentDAO = new EnrollmentDAO();
        }
        public void AddEnrollment(Enrollment enrollment) => _enrollmentDAO.AddEnrollment(enrollment);
        public void DeleteEnrollment(int id) => _enrollmentDAO.DeleteEnrollment(id);
        public Enrollment GetEnrollmentById(int id) => _enrollmentDAO.GetEnrollmentById(id);
        public List<Enrollment> GetAllEnrollments() => _enrollmentDAO.GetAllEnrollments();
        public void UpdateEnrollment(Enrollment enrollment) => _enrollmentDAO.UpdateEnrollment(enrollment);
    }
}

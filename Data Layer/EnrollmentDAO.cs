using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class EnrollmentDAO
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<Enrollment> GetAllEnrollments()
        {
            return _context.Enrollments.ToList();
        }

        public Enrollment GetEnrollmentById(int id)
        {
            return _context.Enrollments.FirstOrDefault(e => e.EnrollmentId == id);
        }
        public void AddEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
        }
        public void UpdateEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            _context.SaveChanges();
        }

        public void DeleteEnrollment(int id)
        {
            var enrollment = _context.Enrollments.FirstOrDefault(e => e.EnrollmentId == id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                _context.SaveChanges();
            }
        }

        public List<Enrollment> GetEnrollmentsByStudentId(int studentId)
      => _context.Enrollments
                 .Where(e => e.StudentId == studentId)
                 .ToList();
    }

}



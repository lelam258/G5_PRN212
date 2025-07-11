using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class InstructorDAO
    {
        private readonly ApplicationDbContext _context;
        public InstructorDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<Instructor> GetAllInstructors()
        {
            return _context.Instructors.ToList();
        }
        public Instructor GetInstructorById(int id)
        {
            return _context.Instructors.FirstOrDefault(i => i.InstructorId == id);
        }
        public void AddInstructor(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            _context.SaveChanges();
        }
        public void UpdateInstructor(Instructor instructor)
        {
            _context.Instructors.Update(instructor);
            _context.SaveChanges();
        }
        public void DeleteInstructor(int id)
        {
            var instructor = _context.Instructors.FirstOrDefault(i => i.InstructorId == id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
                _context.SaveChanges();
            }
        }
    }
}

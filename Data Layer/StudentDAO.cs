using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class StudentDAO
    {
        private readonly ApplicationDbContext _context;
        public StudentDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<Student> GetAllStudents()
        {
            return _context.Students.ToList();
        }
        public Student GetStudentById(int id)
        {
            return _context.Students.FirstOrDefault(s => s.StudentId == id);
        }
        public void AddStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }
        public void UpdateStudent(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }
        public void DeleteStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentId == id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }
        public Student GetStudentByCode(string studentCode)
        {
            return _context.Students.FirstOrDefault(s => s.StudentCode == studentCode);
        }
        public Student Login(string studentCode, string password)
        {
            return _context.Students.FirstOrDefault(s => s.StudentCode == studentCode && s.Password == password);
        }
    }
}

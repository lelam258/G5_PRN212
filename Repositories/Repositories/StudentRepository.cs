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
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentDAO _studentDAO;
        public StudentRepository()
        {
            _studentDAO = new StudentDAO();
        }
        public void AddStudent(Student student) => _studentDAO.AddStudent(student);
        public void DeleteStudent(int id) => _studentDAO.DeleteStudent(id);
        public Student GetStudentById(int id) => _studentDAO.GetStudentById(id);
        public List<Student> GetAllStudents() => _studentDAO.GetAllStudents();
        public void UpdateStudent(Student student) => _studentDAO.UpdateStudent(student);
        Student IStudentRepository.GetStudentByCode(string studentCode) => _studentDAO.GetStudentByCode(studentCode);
        Student IStudentRepository.Login(string studentCode, string password) => _studentDAO.Login(studentCode, password);
    }
}

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
    public class InstructorRepository : IInstructorRepository
    {
        private readonly InstructorDAO _instructorDAO;
        public InstructorRepository()
        {
            _instructorDAO = new InstructorDAO();
        }
        public void AddInstructor(Instructor instructor) => _instructorDAO.AddInstructor(instructor);
        public void DeleteInstructor(int id) => _instructorDAO.DeleteInstructor(id);
        public Instructor GetInstructorById(int id) => _instructorDAO.GetInstructorById(id);
        public List<Instructor> GetAllInstructors() => _instructorDAO.GetAllInstructors();
        public void UpdateInstructor(Instructor instructor) => _instructorDAO.UpdateInstructor(instructor);
    }
}

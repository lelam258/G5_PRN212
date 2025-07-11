using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface IInstructorRepository
    {
        List<Instructor> GetAllInstructors();
        Instructor GetInstructorById(int id);
        void AddInstructor(Instructor instructor);
        void UpdateInstructor(Instructor instructor);
        void DeleteInstructor(int id);
    }
}

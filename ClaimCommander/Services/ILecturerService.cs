using ClaimCommander.Models;
using System.Collections.Generic;

namespace ClaimCommander.Services
{
    public interface ILecturerService
    {
        List<LecturerInfo> GetAllLecturers();
        LecturerInfo? GetLecturer(int id);
        void UpdateLecturer(LecturerInfo lecturer);
        decimal GetLecturerRate(int lecturerId); // Helpful for pre-filling the claim form
    }
}
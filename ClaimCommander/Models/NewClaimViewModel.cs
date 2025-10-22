using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClaimCommander.Models
{
    public class NewClaimViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        [Required(ErrorMessage = "Please select a subject.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter the hours worked.")]
        [Range(0.1, 100, ErrorMessage = "Hours worked must be between 0.1 and 100.")]
        [Display(Name = "Hours Worked")]
        public double HoursWorked { get; set; }

        public string Notes { get; set; }

        // This property will hold the uploaded file
        public IFormFile DocumentFile { get; set; }

        // This property will populate the dropdown
        public List<string> Subjects { get; set; } = new List<string>();
    }
}
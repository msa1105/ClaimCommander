using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the form for submitting a new claim from lecturerpage.txt.
/// </summary>
public class NewClaimViewModel
{
    [Required]
    [Display(Name = "Subject")]
    public int SelectedSubjectId { get; set; }

    [Required]
    [Range(0.1, 100, ErrorMessage = "Hours must be between 0.1 and 100.")]
    [Display(Name = "Teaching Hours")]
    public double HoursWorked { get; set; }

    // This property will hold the list of subjects for the dropdown
    public IEnumerable<SelectListItem> Subjects { get; set; }
}
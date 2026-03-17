using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloud9_2.Pages.Admin.Email;

public class CreateModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        // TODO: mentés adatbázisba

        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "Sablon neve")]
        public string Name { get; set; } = "";

        [Required]
        [Display(Name = "Email tárgy")]
        public string Subject { get; set; } = "";

        [Required]
        [Display(Name = "HTML tartalom")]
        public string BodyHtml { get; set; } = "";

        [Display(Name = "Aktív")]
        public bool IsActive { get; set; } = true;
    }
}
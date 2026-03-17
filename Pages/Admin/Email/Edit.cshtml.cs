using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloud9_2.Pages.Admin.Email;

public class EditModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        Input = new InputModel
        {
            Id = id,
            Name = "Welcome",
            Subject = "Üdvözöljük",
            BodyHtml = "<h1>Welcome</h1>",
            IsActive = true
        };

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        // TODO: update

        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        public int Id { get; set; }

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
        public bool IsActive { get; set; }
    }
}
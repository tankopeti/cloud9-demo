using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloud9_2.Pages.Admin.Email;

public class DeleteModel : PageModel
{
    [BindProperty]
    public EmailTemplateVm Template { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        Template = new EmailTemplateVm
        {
            Id = id,
            Name = "Welcome",
            Subject = "Üdvözöljük"
        };

        return Page();
    }

    public IActionResult OnPost()
    {
        // TODO: delete

        return RedirectToPage("./Index");
    }

    public class EmailTemplateVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Subject { get; set; } = "";
    }
}
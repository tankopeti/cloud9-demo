using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloud9_2.Pages.Admin.Email;

public class DetailsModel : PageModel
{
    public EmailTemplateVm Template { get; set; } = new();

    public void OnGet(int id)
    {
        Template = new EmailTemplateVm
        {
            Id = id,
            Name = "Welcome",
            Subject = "Üdvözöljük",
            BodyHtml = "<h1>Welcome</h1>",
            IsActive = true
        };
    }

    public class EmailTemplateVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Subject { get; set; } = "";
        public string BodyHtml { get; set; } = "";
        public bool IsActive { get; set; }
    }
}
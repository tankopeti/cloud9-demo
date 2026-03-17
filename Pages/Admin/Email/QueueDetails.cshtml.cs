using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloud9_2.Pages.Admin.Email;

public class QueueDetailsModel : PageModel
{
    public QueueItemVm Item { get; set; } = new();

    public void OnGet(int id)
    {
        Item = new QueueItemVm
        {
            Id = id,
            To = "user@test.com",
            Subject = "Welcome",
            Status = "Pending",
            Body = "<h1>Hello</h1>"
        };
    }

    public class QueueItemVm
    {
        public int Id { get; set; }
        public string To { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Status { get; set; } = "";
        public string Body { get; set; } = "";
    }
}
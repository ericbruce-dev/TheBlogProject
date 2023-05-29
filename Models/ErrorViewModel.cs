using System;

namespace TheBlogProject.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; } = default!;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

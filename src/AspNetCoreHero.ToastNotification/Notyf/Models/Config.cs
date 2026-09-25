using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreHero.ToastNotification.Notyf.Models
{
    public class Config
    {
        public string? type { get; set; }
        public string? background { get; set; }
        public string? className { get; set; }
        public Icon? icon { get; set; }
    }
}


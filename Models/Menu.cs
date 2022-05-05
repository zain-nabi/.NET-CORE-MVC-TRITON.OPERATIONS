using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Triton.Operations.Models
{
    public class MainMenu
    {
        [Required]
        public string MainTitle { get; set; }

        [Required]
        public string SubTitle { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }
    }

    public class ParentMenu
    {
        [Required]
        public string Title { get; set; }

        public string Icon { get; set; }
    }

    public class SubMenu
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Controller { get; set; }

        public string Action { get; set; }

        public string Role { get; set; }
    }
}

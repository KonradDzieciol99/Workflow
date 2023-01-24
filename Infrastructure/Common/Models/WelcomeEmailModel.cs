using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Models
{
    public class WelcomeEmailModel
    {
        public string Name { get; set; }
        public string ConfirmationLink { get; set; }
    }
}

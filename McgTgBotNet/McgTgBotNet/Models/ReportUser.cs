using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McgTgBotNet.Models
{
    public partial class ReportUser
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
    }
}

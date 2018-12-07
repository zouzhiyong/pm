using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class T_Status
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string remark { get; set; }
    }
}
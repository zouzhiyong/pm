using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class HR_report_select: condition
    {
        [Key]
        public Int64 ID { get; set; }
        public string t_type { get; set; }        
        public string t_select_name { get; set; }
        public string t_select_value { get; set; }
    }
}
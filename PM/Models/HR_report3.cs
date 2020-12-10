using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class HR_report3: condition
    {
        [Key]
        public Int64 ID { get; set; }
        public string t_year_month { get; set; }
        public string t_ws { get; set; }
        public string t_sku { get; set; }
        public Int64 t_sales_number { get; set; }
        public Int64 t_sales_je { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class StuInfoDBContext : DbContext
    {
        public StuInfoDBContext()
            : base("DefaultConnection")
        {
        }
    }

    public class TestDBContext : DbContext
    {
        public TestDBContext()
            : base("TestConnection")
        {
        }

        public DbSet<T_WS> T_WS { get; set; }
        public DbSet<T_ImplementPersonnel> T_ImplementPersonnel { get; set; }
        public DbSet<T_Status> T_Status { get; set; }
        public DbSet<HR_report1> HR_report1 { get; set; }
        public DbSet<HR_report2> HR_report2 { get; set; }
        public DbSet<HR_report3> HR_report3 { get; set; }
        public DbSet<HR_report_select> HR_report_select { get; set; }
        public DbSet<HR_report_poptype> HR_report_poptype { get; set; }
        public DbSet<HR_report_list> HR_report_list { get; set; }
    }    
}
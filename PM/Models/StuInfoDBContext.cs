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
    }    
}
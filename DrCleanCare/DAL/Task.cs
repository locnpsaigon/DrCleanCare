using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.DAL
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }

        public string TaskName { get; set; }

        public string Description { get; set; }

    }
}
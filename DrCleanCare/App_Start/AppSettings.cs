using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace DrCleanCare
{
    public class AppSettings
    {
        public static int DEFAULT_PAGE_SIZE = 25;

        public static void LoadConfigurations()
        {
            // Load default paging size
            int pagingSize;
            if (int.TryParse(ConfigurationManager.AppSettings["DEFAULT_PAGE_SIZE"], out pagingSize))
            {
                DEFAULT_PAGE_SIZE = pagingSize;
            }
        }
    }
}
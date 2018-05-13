using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{
    public class AddCategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage="Tên loại sản phẩm không được rỗng")]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public HttpPostedFileBase Icon { get; set; }

        public string IconURL { get; set; }

        [Required(ErrorMessage="Vui lòng nhập thứ tự sắp xếp")]
        public int OrderNumber { get; set; }

        public bool Displayed { get; set; }
    }
}
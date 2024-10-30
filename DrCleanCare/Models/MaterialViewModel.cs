using System.ComponentModel.DataAnnotations;

namespace DrCleanCare.Models
{
    public class AddMaterialViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên nguyên liệu")]
        public string MaterialName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đơn vị tính")]
        public string QuantityPerUnit { get; set; }
    }

    public class EditMaterialViewModel
    {
        [Key]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nguyên liệu")]
        public string MaterialName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đơn vị tính")]
        public string QuantityPerUnit { get; set; }

        public string UnitInStock { get; set; }
    }

    public class AddMaterialImportViewModel
    {
        [Required(ErrorMessage = "Chọn ngày nhập nguyên liệu")]
        public string MaterialImportDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nguyên liệu")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho nhập")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public string Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public string Price { get; set; }

        public string Notes { get; set; }
    }

    public class EditMaterialImportViewModel
    {
        [Key]
        public int MaterialImportId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày nhập nguyên liệu")]
        public string MaterialImportDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nguyên liệu")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho nhập")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public string Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public string Price { get; set; }

        public string Notes { get; set; }
    }

    public class AddMaterialExportViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn ngày xuất sử dụng")]
        public string MaterialExportDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nguyên liệu")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho xuất")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số lượng")]
        public string Quantity { get; set; }

        public string Notes { get; set; }
    }

    public class EditMaterialExportViewModel
    {
        [Key]
        public int MaterialExportId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày xuất sử dụng")]
        public string MaterialExportDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nguyên liệu")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho xuất")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số lượng")]
        public string Quantity { get; set; }

        public string Notes { get; set; }
    }

    public class AddMaterialInStockViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn nguyên liệu")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public string Quantity { get; set; }
    }

    public class EditMaterialInStockViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn nguyên liệu")]
        public int MaterialId { get; set; }

        public string MaterialName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho")]
        public int StockId { get; set; }

        public string StockName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public string Quantity { get; set; }
    }
}
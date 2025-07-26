using System.Text.RegularExpressions;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ImportMedicineEX;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Ultility.Validation
{
    public static class MedicineImportExcelValidation
    {
        // Regex cho phép tiếng Việt, số, khoảng trắng, dấu chấm và dấu phẩy
        private static readonly Regex TiengVietRegex = new(@"^[a-zA-Z0-9À-ỹ\s\.,]*$");

        public static void ValidateRow(MedicineImportDetailRequest detail, int row)
        {
            var requiredFields = new Dictionary<string, string>
            {
                { "Tên thuốc", detail.MedicineName },
                { "Đơn vị", detail.UnitName },
                { "Liều dùng", detail.Dosage },
                { "Thành phần", detail.Ingredients },
                { "Danh mục", detail.CategoryName },
                { "Hướng dẫn bảo quản", detail.StorageInstructions },
                { "Mô tả", detail.MedicineDescription },
                { "Mô tả chi tiết", detail.MedicineDetailDescription },
                { "Cảnh báo", detail.Waring }
            };

            foreach (var field in requiredFields)
            {
                if (string.IsNullOrWhiteSpace(field.Value))
                    throw new Exception($"Dòng {row}: {field.Key} không được để trống.");

                if (!TiengVietRegex.IsMatch(field.Value))
                    throw new Exception($"Dòng {row}: {field.Key} chứa ký tự không hợp lệ.");
            }

            
            if (detail.Quantity <= 0)
                throw new Exception($"Dòng {row}: Số lượng phải lớn hơn 0.");

            if (detail.UnitPrice <= 0)
                throw new Exception($"Dòng {row}: Giá bán phải lớn hơn 0.");

            
            if (detail.ManufactureDate >= detail.ExpiryDate)
                throw new Exception($"Dòng {row}: Ngày sản xuất phải nhỏ hơn ngày hết hạn.");

            if (detail.ExpiryDate <= DateTime.UtcNow.Date)
                throw new Exception($"Dòng {row}: Ngày hết hạn phải lớn hơn ngày hiện tại.");
        }
    }
}

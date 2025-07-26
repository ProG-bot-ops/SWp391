using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
using System.Text.RegularExpressions;

namespace SWP391_SE1914_ManageHospital.Ultility.Validation;

public class PatientUpdateValidator
{
    private static readonly Regex PhoneRegex = new Regex(@"^0\d{9}$");
    private static readonly Regex NameRegex = new Regex(@"^[A-Za-zÀ-Ỹà-ỹ\s]{2,50}$");

    private static readonly string[] _validBloodTypes = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
    public static void Validate(PatientUpdate update)
    {
        if (update == null)
            throw new ArgumentNullException(nameof(update), "Dữ liệu cập nhật không được để trống");

        if (string.IsNullOrWhiteSpace(update.Name) || !NameRegex.IsMatch(update.Name))
            throw new ArgumentException("Tên bệnh nhân không hợp lệ", nameof(update.Name));

        if (update.Dob >= DateTime.Today)
            throw new ArgumentException("Ngày sinh phải nhỏ hơn ngày hiện tại");

        
        if (string.IsNullOrEmpty(update.BloodType) || !_validBloodTypes.Contains(update.BloodType))
            throw new ArgumentException("Nhóm máu không hợp lệ");

        if (string.IsNullOrEmpty(update.Phone) || !PhoneRegex.IsMatch(update.Phone))
            throw new ArgumentException("Số điện thoại không hợp lệ (phải bắt đầu bằng 0 và có 10 chữ số)");

        // Kiểm tra số điện thoại người liên hệ khẩn cấp
        if (!string.IsNullOrEmpty(update.EmergencyContact))
        {
            if (!PhoneRegex.IsMatch(update.EmergencyContact))
                throw new ArgumentException("Số điện thoại người liên hệ khẩn cấp không hợp lệ (phải bắt đầu bằng 0 và có 10 chữ số)");

            if (update.EmergencyContact == update.Phone)
                throw new ArgumentException("Số điện thoại người liên hệ khẩn cấp không được trùng với số chính");
        }
        else
        {
            throw new Exception("Số điện thoại khẩn cấp không được để trống");
        }

    }
}

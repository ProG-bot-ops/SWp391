using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Prescription;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IPrescriptionMapper
    {
        PrescriptionResponseDTO MapToResponse(Prescription prescription);  // Ánh xạ từ entity Prescription sang DTO PrescriptionResponseDTO
        Prescription MapToEntity(PrescriptionRequest request);  // Ánh xạ từ DTO PrescriptionRequest sang entity Prescription
        void MapToExistingEntity(PrescriptionRequest request, Prescription prescription);  // Cập nhật entity Prescription với DTO PrescriptionRequest
    }

}

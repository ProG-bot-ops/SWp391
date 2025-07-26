using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.Helps;
using System;
using System.Collections.Generic;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IPatientFilterMapper
    {
        /// Chuyển đổi từ Patient entity sang PatientFilterResponse
        PatientFilterResponse EntityToResponse(Patient entity, DateTime examinationTime, string appointmentStatus);

        /// Chuyển đổi danh sách Patient thành danh sách PatientFilterResponse
        List<PatientFilterResponse> ListEntityToResponse(List<PatientAppointmentData> patientDataList);
    }
}
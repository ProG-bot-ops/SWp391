using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.Helps;
using SWP391_SE1914_ManageHospital.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class PatientFilterMapper : IPatientFilterMapper
    {
        public PatientFilterResponse EntityToResponse(Patient entity, DateTime examinationTime, string appointmentStatus)
        {
            Console.WriteLine($"=== PatientFilterMapper.EntityToResponse ===");
            Console.WriteLine($"Patient: {entity?.Name ?? "null"}, Time: {examinationTime:yyyy-MM-dd HH:mm}, Status: {appointmentStatus ?? "null"}");

            if (entity == null)
            {
                Console.WriteLine("Patient entity is null, returning default response");
                return new PatientFilterResponse
                {
                    Id = 0,
                    Name = "Unknown",
                    ExaminationTime = examinationTime,
                    AppointmentStatus = appointmentStatus ?? "Unknown"
                };
            }

            var response = new PatientFilterResponse
            {
                Id = entity.Id,
                Name = entity.Name ?? "Unknown",
                ExaminationTime = examinationTime,
                AppointmentStatus = appointmentStatus ?? "Unknown"
            };

            Console.WriteLine($"Mapped response: ID={response.Id}, Name={response.Name}, Time={response.ExaminationTime:yyyy-MM-dd HH:mm}, Status={response.AppointmentStatus}");
            return response;
        }

        // ✅ THÊM METHOD NÀY
        public List<PatientFilterResponse> ListEntityToResponse(List<PatientAppointmentData> patientDataList)
        {
            Console.WriteLine($"=== PatientFilterMapper.ListEntityToResponse ===");
            Console.WriteLine($"Input list count: {patientDataList?.Count ?? 0}");

            if (patientDataList == null || patientDataList.Count == 0)
            {
                Console.WriteLine("PatientDataList is null or empty, returning empty list");
                return new List<PatientFilterResponse>();
            }

            var result = new List<PatientFilterResponse>();

            foreach (var patientData in patientDataList)
            {
                Console.WriteLine($"Processing: Patient={patientData.Patient?.Name ?? "null"}, Time={patientData.ExaminationTime:yyyy-MM-dd HH:mm}, Status={patientData.AppointmentStatus}");

                var response = EntityToResponse(patientData.Patient, patientData.ExaminationTime, patientData.AppointmentStatus);
                result.Add(response);
            }

            Console.WriteLine($"Mapped result count: {result.Count}");
            return result;
        }
    }
}
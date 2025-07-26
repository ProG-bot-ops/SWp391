-- Sample data for Hospital Management System
-- Insert clinics, departments, doctors, services and other data

USE hospitalmanagement;

-- Disable safe update mode temporarily
SET SQL_SAFE_UPDATES = 0;

-- Clear data in reverse order of dependencies (child tables first, then parent tables)

-- Clear junction tables and detail tables first
DELETE FROM payment_invoices;
DELETE FROM invoicedetail;
DELETE FROM prescriptiondetails;
DELETE FROM nurse_appointments;
DELETE FROM doctor_appointment;
DELETE FROM supply_inventories;
DELETE FROM feedbacks;
DELETE FROM notifications;
DELETE FROM shift_request;
DELETE FROM doctor_shifts;

-- Clear main entity tables
DELETE FROM appointments;
DELETE FROM payments;
DELETE FROM invoices;
DELETE FROM medical_records;
DELETE FROM prescriptions;
DELETE FROM insurances;
DELETE FROM medicine_inventories;
DELETE FROM medicine_import_details;
DELETE FROM medicine_imports;
DELETE FROM medicine_detail;
DELETE FROM supplies;
DELETE FROM notifications;
DELETE FROM feedbacks;

-- Clear user-related tables
DELETE FROM user_roles;
DELETE FROM patients;
DELETE FROM nurses;
DELETE FROM receptions;
DELETE FROM doctors;

-- Clear core entity tables
DELETE FROM services;
DELETE FROM medicines;
DELETE FROM medicinecategories;
DELETE FROM units;
DELETE FROM diseasedetail;
DELETE FROM diseases;
DELETE FROM suppliers;

-- Clear main entity tables
DELETE FROM departments;
DELETE FROM clinics;
DELETE FROM roles;
DELETE FROM users;

-- Reset auto-increment counters
ALTER TABLE payment_invoices AUTO_INCREMENT = 1;
ALTER TABLE invoicedetail AUTO_INCREMENT = 1;
ALTER TABLE prescriptiondetails AUTO_INCREMENT = 1;
ALTER TABLE nurse_appointments AUTO_INCREMENT = 1;
ALTER TABLE doctor_appointment AUTO_INCREMENT = 1;
ALTER TABLE supply_inventories AUTO_INCREMENT = 1;
ALTER TABLE feedbacks AUTO_INCREMENT = 1;
ALTER TABLE notifications AUTO_INCREMENT = 1;
ALTER TABLE shift_request AUTO_INCREMENT = 1;
ALTER TABLE doctor_shifts AUTO_INCREMENT = 1;
ALTER TABLE appointments AUTO_INCREMENT = 1;
ALTER TABLE payments AUTO_INCREMENT = 1;
ALTER TABLE invoices AUTO_INCREMENT = 1;
ALTER TABLE medical_records AUTO_INCREMENT = 1;
ALTER TABLE prescriptions AUTO_INCREMENT = 1;
ALTER TABLE insurances AUTO_INCREMENT = 1;
ALTER TABLE medicine_inventories AUTO_INCREMENT = 1;
ALTER TABLE medicine_import_details AUTO_INCREMENT = 1;
ALTER TABLE medicine_imports AUTO_INCREMENT = 1;
ALTER TABLE medicine_detail AUTO_INCREMENT = 1;
ALTER TABLE supplies AUTO_INCREMENT = 1;
ALTER TABLE patients AUTO_INCREMENT = 1;
ALTER TABLE nurses AUTO_INCREMENT = 1;
ALTER TABLE receptions AUTO_INCREMENT = 1;
ALTER TABLE doctors AUTO_INCREMENT = 1;
ALTER TABLE services AUTO_INCREMENT = 1;
ALTER TABLE medicines AUTO_INCREMENT = 1;
ALTER TABLE medicinecategories AUTO_INCREMENT = 1;
ALTER TABLE units AUTO_INCREMENT = 1;
ALTER TABLE diseasedetail AUTO_INCREMENT = 1;
ALTER TABLE diseases AUTO_INCREMENT = 1;
ALTER TABLE suppliers AUTO_INCREMENT = 1;
ALTER TABLE departments AUTO_INCREMENT = 1;
ALTER TABLE clinics AUTO_INCREMENT = 1;
ALTER TABLE roles AUTO_INCREMENT = 1;
ALTER TABLE users AUTO_INCREMENT = 1;

-- Re-enable safe update mode
-- SET SQL_SAFE_UPDATES = 1;

SELECT 'All data cleared successfully!' as message; 

-- Insert sample clinics
INSERT INTO clinics (Id, Status, Type, Name, Code, Address, Email, ImageUrl, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 0, 1, 'Phòng khám Đa khoa Miracle', 'CL001', '123 Đường Nguyễn Huệ, Quận 1, TP.HCM', 'miracle_clinic@gcare.com', '/assets/images/pages/clinic-2.webp', NOW(), NOW(), 'System', 'System'),
(2, 0, 1, 'Phòng khám Đa khoa Valley', 'CL002', '456 Đường Võ Văn Tần, Quận 3, TP.HCM', 'valley_clinic@yopmail.com', '/assets/images/pages/clinic-1.webp', NOW(), NOW(), 'System', 'System'),
(3, 0, 2, 'Phòng khám Chuyên khoa Tim mạch', 'CL003', '789 Đường Lê Lợi, Quận 1, TP.HCM', 'cardio_clinic@gcare.com', '/assets/images/pages/clinic-3.webp', NOW(), NOW(), 'System', 'System');

-- Insert sample departments for Clinic 1 (Miracle)
INSERT INTO departments (Id, Description, TotalAmountOfPeople, Status, ClinicId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Khoa Nội tổng hợp - Chuyên khám và điều trị các bệnh nội khoa', 15, 0, 1, 'Khoa Nội', 'DEP001', NOW(), NOW(), 'System', 'System'),
(2, 'Khoa Ngoại tổng hợp - Thực hiện các phẫu thuật cơ bản', 12, 0, 1, 'Khoa Ngoại', 'DEP002', NOW(), NOW(), 'System', 'System'),
(3, 'Khoa Nhi - Chuyên khám và điều trị bệnh nhi', 10, 0, 1, 'Khoa Nhi', 'DEP003', NOW(), NOW(), 'System', 'System'),
(4, 'Khoa Sản phụ khoa - Chuyên về sản khoa và phụ khoa', 8, 0, 1, 'Khoa Sản', 'DEP004', NOW(), NOW(), 'System', 'System');

-- Insert sample departments for Clinic 2 (Valley)
INSERT INTO departments (Id, Description, TotalAmountOfPeople, Status, ClinicId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(5, 'Khoa Nội tổng hợp - Chuyên khám và điều trị các bệnh nội khoa', 12, 0, 2, 'Khoa Nội', 'DEP005', NOW(), NOW(), 'System', 'System'),
(6, 'Khoa Ngoại tổng hợp - Thực hiện các phẫu thuật cơ bản', 10, 0, 2, 'Khoa Ngoại', 'DEP006', NOW(), NOW(), 'System', 'System'),
(7, 'Khoa Da liễu - Chuyên điều trị các bệnh về da', 6, 0, 2, 'Khoa Da liễu', 'DEP007', NOW(), NOW(), 'System', 'System');

-- Insert sample departments for Clinic 3 (Tim mạch)
INSERT INTO departments (Id, Description, TotalAmountOfPeople, Status, ClinicId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(8, 'Khoa Tim mạch chuyên sâu - Chuyên điều trị các bệnh tim mạch', 8, 0, 3, 'Khoa Tim mạch', 'DEP008', NOW(), NOW(), 'System', 'System'),
(9, 'Khoa Phẫu thuật tim - Thực hiện các phẫu thuật tim mạch', 6, 0, 3, 'Khoa Phẫu thuật tim', 'DEP009', NOW(), NOW(), 'System', 'System');

-- Insert sample users for doctors, nurses, patients, receptions
INSERT INTO users (Id, Email, Password, Status) VALUES
(1, 'dr.nguyen@miracle.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(2, 'dr.tran@miracle.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(3, 'dr.le@miracle.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(4, 'dr.pham@miracle.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(5, 'dr.hoang@valley.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(6, 'dr.vu@valley.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(7, 'dr.dang@valley.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(8, 'dr.bui@cardio.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(9, 'dr.do@cardio.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(10, 'nurse.alice@miracle.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(11, 'nurse.bob@valley.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(12, 'nurse.carol@cardio.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(13, 'patient.david@email.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(14, 'patient.lisa@email.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(15, 'patient.james@email.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(16, 'reception.mary@miracle.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(17, 'reception.john@valley.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0),
(18, 'admin@hospital.com', '$2a$11$sWlaiwPHy8JHIlFvqDVMA.goajHtVLfYGOpR.Kgrc.AEJkIye5DDi', 0);

-- Insert sample doctors for Clinic 1 (Miracle)
INSERT INTO doctors (Id, Gender, Dob, CCCD, Phone, ImageURL, LicenseNumber, YearOfExperience, WorkingHours, Status, UserId, DepartmentId, ClinicId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 1, '1980-05-15', '123456789012', '0901234567', 'doctor-1.jpg', 'BS001', 15.0, 8.0, 0, 1, 1, 1, 'Bác sĩ Nguyễn Văn An', 'DOC001', NOW(), NOW(), 'System', 'System'),
(2, 2, '1985-08-20', '123456789013', '0901234568', 'doctor-2.jpg', 'BS002', 12.0, 8.0, 0, 2, 2, 1, 'Bác sĩ Trần Thị Bình', 'DOC002', NOW(), NOW(), 'System', 'System'),
(3, 1, '1982-03-10', '123456789014', '0901234569', 'doctor-3.jpg', 'BS003', 10.0, 8.0, 0, 3, 3, 1, 'Bác sĩ Lê Văn Cường', 'DOC003', NOW(), NOW(), 'System', 'System'),
(4, 2, '1988-12-25', '123456789015', '0901234570', 'doctor-4.jpg', 'BS004', 8.0, 8.0, 0, 4, 4, 1, 'Bác sĩ Phạm Thị Dung', 'DOC004', NOW(), NOW(), 'System', 'System');

-- Insert sample doctors for Clinic 2 (Valley)
INSERT INTO doctors (Id, Gender, Dob, CCCD, Phone, ImageURL, LicenseNumber, YearOfExperience, WorkingHours, Status, UserId, DepartmentId, ClinicId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(5, 1, '1983-07-18', '123456789016', '0901234571', 'doctor-5.jpg', 'BS005', 14.0, 8.0, 0, 5, 5, 2, 'Bác sĩ Hoàng Văn Em', 'DOC005', NOW(), NOW(), 'System', 'System'),
(6, 2, '1986-11-30', '123456789017', '0901234572', 'doctor-6.jpg', 'BS006', 11.0, 8.0, 0, 6, 6, 2, 'Bác sĩ Vũ Thị Phương', 'DOC006', NOW(), NOW(), 'System', 'System'),
(7, 1, '1984-04-22', '123456789018', '0901234573', 'doctor-7.jpg', 'BS007', 9.0, 8.0, 0, 7, 7, 2, 'Bác sĩ Đặng Văn Giang', 'DOC007', NOW(), NOW(), 'System', 'System');

-- Insert sample doctors for Clinic 3 (Tim mạch)
INSERT INTO doctors (Id, Gender, Dob, CCCD, Phone, ImageURL, LicenseNumber, YearOfExperience, WorkingHours, Status, UserId, DepartmentId, ClinicId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(8, 1, '1978-09-12', '123456789019', '0901234574', 'doctor-8.jpg', 'BS008', 20.0, 8.0, 0, 8, 8, 3, 'Bác sĩ Bùi Văn Hùng', 'DOC008', NOW(), NOW(), 'System', 'System'),
(9, 2, '1981-06-05', '123456789020', '0901234575', 'doctor-9.jpg', 'BS009', 16.0, 8.0, 0, 9, 9, 3, 'Bác sĩ Đỗ Thị Lan', 'DOC009', NOW(), NOW(), 'System', 'System');

-- Insert sample nurses
INSERT INTO nurses (Id, Name, Code, Gender, Dob, CCCD, Phone, ImageURL, Status, UserId, DepartmentId, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Y tá Alice Wilson', 'NUR001', 2, '1988-06-18', '123456789021', '0901234576', 'nurse-1.jpg', 0, 10, 1, NOW(), NOW(), 'System', 'System'),
(2, 'Y tá Bob Anderson', 'NUR002', 1, '1992-11-05', '123456789022', '0901234577', 'nurse-2.jpg', 0, 11, 5, NOW(), NOW(), 'System', 'System'),
(3, 'Y tá Carol Taylor', 'NUR003', 2, '1985-09-12', '123456789023', '0901234578', 'nurse-3.jpg', 0, 12, 8, NOW(), NOW(), 'System', 'System');

-- Insert sample patients
INSERT INTO patients (Id, Name, Code, Gender, Dob, CCCD, Phone, EmergencyContact, Address, InsuranceNumber, Allergies, Status, BloodType, ImageURL, UserId, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Bệnh nhân David Miller', 'PAT001', 1, '1985-03-15', '123456789024', '0901234579', 'Jane Miller', '258 Spruce St, City', 'INS001', 'Penicillin', 0, 'A+', 'patient-1.jpg', 13, NOW(), NOW(), 'System', 'System'),
(2, 'Bệnh nhân Lisa Garcia', 'PAT002', 2, '1990-07-22', '123456789025', '0901234580', 'Carlos Garcia', '369 Willow Ave, City', 'INS002', 'None', 0, 'O+', 'patient-2.jpg', 14, NOW(), NOW(), 'System', 'System'),
(3, 'Bệnh nhân James Rodriguez', 'PAT003', 1, '1978-11-08', '123456789026', '0901234581', 'Maria Rodriguez', '741 Aspen Rd, City', 'INS003', 'Sulfa drugs', 0, 'B+', 'patient-3.jpg', 15, NOW(), NOW(), 'System', 'System');

-- Insert sample receptions
INSERT INTO receptions (Id, Name, Code, Gender, Dob, CCCD, Phone, ImageURL, Status, UserId, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Lễ tân Mary Johnson', 'REC001', 2, '1990-01-01', '123456789027', '0901234582', 'reception-1.jpg', 0, 16, NOW(), NOW(), 'System', 'System'),
(2, 'Lễ tân John Smith', 'REC002', 1, '1992-05-15', '123456789028', '0901234583', 'reception-2.jpg', 0, 17, NOW(), NOW(), 'System', 'System');

-- Insert roles
INSERT INTO roles (Id, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Admin', 'ADMIN', NOW(), NOW(), 'System', 'System'),
(2, 'Doctor', 'DOCTOR', NOW(), NOW(), 'System', 'System'),
(3, 'Nurse', 'NURSE', NOW(), NOW(), 'System', 'System'),
(4, 'Patient', 'PATIENT', NOW(), NOW(), 'System', 'System'),
(5, 'Reception', 'RECEPTION', NOW(), NOW(), 'System', 'System');

-- Assign roles to users
INSERT INTO user_roles (RoleId, UserId) VALUES
(1, 18), -- admin
(2, 1), (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (2, 7), (2, 8), (2, 9), -- doctors
(3, 10), (3, 11), (3, 12), -- nurses
(4, 13), (4, 14), (4, 15), -- patients
(5, 16), (5, 17); -- receptions

-- Insert sample services
INSERT INTO services (Id, Name, Code, Description, ImageUrl, Price, Status, DepartmentId, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Khám tổng quát', 'SER001', 'Khám sức khỏe tổng quát - Kiểm tra toàn diện sức khỏe', 'service-1.jpg', 200000.00, 0, 1, NOW(), NOW(), 'System', 'System'),
(2, 'Khám chuyên khoa nội', 'SER002', 'Khám chuyên khoa nội - Điều trị các bệnh nội khoa', 'service-2.jpg', 300000.00, 0, 1, NOW(), NOW(), 'System', 'System'),
(3, 'Phẫu thuật ngoại', 'SER003', 'Phẫu thuật ngoại cơ bản - Các phẫu thuật thông thường', 'service-3.jpg', 500000.00, 0, 2, NOW(), NOW(), 'System', 'System'),
(4, 'Khám nhi', 'SER004', 'Khám bệnh nhi - Chuyên khám và điều trị trẻ em', 'service-4.jpg', 250000.00, 0, 3, NOW(), NOW(), 'System', 'System'),
(5, 'Khám sản phụ khoa', 'SER005', 'Khám sản phụ khoa - Chuyên về sản khoa và phụ khoa', 'service-5.jpg', 350000.00, 0, 4, NOW(), NOW(), 'System', 'System'),
(6, 'Khám da liễu', 'SER006', 'Khám chuyên khoa da liễu - Điều trị các bệnh về da', 'service-6.jpg', 280000.00, 0, 7, NOW(), NOW(), 'System', 'System'),
(7, 'Siêu âm tim', 'SER007', 'Siêu âm tim mạch - Chẩn đoán bệnh tim mạch', 'service-7.jpg', 400000.00, 0, 8, NOW(), NOW(), 'System', 'System'),
(8, 'Điện tâm đồ', 'SER008', 'Điện tâm đồ - Kiểm tra hoạt động tim', 'service-8.jpg', 150000.00, 0, 8, NOW(), NOW(), 'System', 'System'),
(9, 'Phẫu thuật tim', 'SER009', 'Phẫu thuật tim mạch - Các phẫu thuật tim chuyên sâu', 'service-9.jpg', 2000000.00, 0, 9, NOW(), NOW(), 'System', 'System'),
(10, 'Khám ngoại chung', 'SER010', 'Khám ngoại chung - Khám và tư vấn ngoại khoa', 'service-10.jpg', 250000.00, 0, 2, NOW(), NOW(), 'System', 'System'),
(11, 'Khám ngoại chung Valley', 'SER011', 'Khám ngoại chung tại Valley - Khám và tư vấn ngoại khoa', 'service-11.jpg', 250000.00, 0, 6, NOW(), NOW(), 'System', 'System'),
(12, 'Khám nội Valley', 'SER012', 'Khám nội tại Valley - Khám và điều trị nội khoa', 'service-12.jpg', 280000.00, 0, 5, NOW(), NOW(), 'System', 'System');

-- Insert sample units
INSERT INTO units (Id, Name, Status) VALUES
(1, 'Viên', 0),
(2, 'Chai', 0),
(3, 'Hộp', 0),
(4, 'Lần', 0),
(5, 'Buổi', 0);

-- Insert sample medicine categories
INSERT INTO medicinecategories (Id, Description, ImageUrl, Status, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Thuốc kháng sinh', '/assets/images/medicines/antibiotics.jpg', 0, 'Kháng sinh', 'MEDCAT001', NOW(), NOW(), 'System', 'System'),
(2, 'Thuốc giảm đau', '/assets/images/medicines/painkillers.jpg', 0, 'Giảm đau', 'MEDCAT002', NOW(), NOW(), 'System', 'System'),
(3, 'Thuốc tim mạch', '/assets/images/medicines/cardio.jpg', 0, 'Tim mạch', 'MEDCAT003', NOW(), NOW(), 'System', 'System');

-- Insert sample medicines
INSERT INTO medicines (Id, ImageUrl, Status, Description, UnitId, Prescribed, Dosage, MedicineCategoryId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, '/assets/images/medicines/paracetamol.jpg', 0, 'Thuốc giảm đau, hạ sốt', 1, 1, '500mg x 2 viên/lần', 2, 'Paracetamol', 'MED001', NOW(), NOW(), 'System', 'System'),
(2, '/assets/images/medicines/amoxicillin.jpg', 0, 'Kháng sinh điều trị nhiễm khuẩn', 1, 1, '500mg x 3 viên/ngày', 1, 'Amoxicillin', 'MED002', NOW(), NOW(), 'System', 'System'),
(3, '/assets/images/medicines/aspirin.jpg', 0, 'Thuốc tim mạch, chống đông máu', 1, 1, '100mg x 1 viên/ngày', 3, 'Aspirin', 'MED003', NOW(), NOW(), 'System', 'System');

-- Insert sample diseases
INSERT INTO diseases (Id, Description, Status, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Bệnh cảm cúm thông thường', 0, 'Cảm cúm', 'DIS001', NOW(), NOW(), 'System', 'System'),
(2, 'Bệnh viêm phổi', 0, 'Viêm phổi', 'DIS002', NOW(), NOW(), 'System', 'System'),
(3, 'Bệnh tim mạch', 0, 'Tim mạch', 'DIS003', NOW(), NOW(), 'System', 'System');

-- Insert sample disease details
INSERT INTO diseasedetail (Id, Description, Status, DiseaseId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Cảm cúm nhẹ', 0, 1, 'Cảm cúm nhẹ', 'DISDET001', NOW(), NOW(), 'System', 'System'),
(2, 'Cảm cúm nặng', 0, 1, 'Cảm cúm nặng', 'DISDET002', NOW(), NOW(), 'System', 'System'),
(3, 'Viêm phổi nhẹ', 0, 2, 'Viêm phổi nhẹ', 'DISDET003', NOW(), NOW(), 'System', 'System');

-- Insert sample suppliers
INSERT INTO suppliers (Id, Phone, Email, Address, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, '028-1111-2222', 'supplier1@email.com', '123 Đường ABC, Quận 1, TP.HCM', 'Công ty Dược phẩm ABC', 'SUP001', NOW(), NOW(), 'System', 'System'),
(2, '028-2222-3333', 'supplier2@email.com', '456 Đường XYZ, Quận 2, TP.HCM', 'Công ty Dược phẩm XYZ', 'SUP002', NOW(), NOW(), 'System', 'System');

-- Insert sample medicine imports
INSERT INTO medicine_imports (Id, Notes, SupplierId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Nhập thuốc tháng 1/2024', 1, 'Đơn nhập thuốc tháng 1', 'IMP001', NOW(), NOW(), 'System', 'System'),
(2, 'Nhập thuốc tháng 2/2024', 2, 'Đơn nhập thuốc tháng 2', 'IMP002', NOW(), NOW(), 'System', 'System');

-- Insert sample medicine import details
INSERT INTO medicine_import_details (Id, ImportId, MedicineId, BatchNumber, Quantity, UnitPrice, ManufactureDate, ExpiryDate, UnitId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 1, 1, 'BATCH001', 1000, 5000.00, '2024-01-01', '2026-01-01', 1, 'Chi tiết nhập Paracetamol', 'IMPDET001', NOW(), NOW(), 'System', 'System'),
(2, 1, 2, 'BATCH002', 500, 15000.00, '2024-01-01', '2025-01-01', 1, 'Chi tiết nhập Amoxicillin', 'IMPDET002', NOW(), NOW(), 'System', 'System');

-- Insert sample medicine inventories
INSERT INTO medicine_inventories (Id, Quantity, BatchNumber, UnitPrice, ImportDate, ExpiryDate, Status, MedicineId, ImportDetailId) VALUES
(1, 1000, 'BATCH001', 5000.00, NOW(), '2026-01-01', 0, 1, 1),
(2, 500, 'BATCH002', 15000.00, NOW(), '2025-01-01', 0, 2, 2);

-- Insert sample medicine details
INSERT INTO medicine_detail (Id, MedicineId, Ingredients, ExpiryDate, Manufacturer, Warning, StorageInstructions, Status, CreateDate, UpdateDate, CreateBy, UpdateBy, Description) VALUES
(1, 1, 'Paracetamol 500mg', '2026-01-01', 'Công ty Dược phẩm ABC', 'Không dùng quá liều', 'Bảo quản nơi khô ráo, tránh ánh nắng', 0, NOW(), NOW(), 'System', 'System', 'Thuốc giảm đau, hạ sốt'),
(2, 2, 'Amoxicillin 500mg', '2025-01-01', 'Công ty Dược phẩm XYZ', 'Có thể gây dị ứng', 'Bảo quản trong tủ lạnh', 0, NOW(), NOW(), 'System', 'System', 'Kháng sinh điều trị nhiễm khuẩn');

-- Insert sample insurances
INSERT INTO insurances (Id, Description, Status, CoveragePercent, StartDate, EndDate, PatientId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Bảo hiểm y tế cơ bản', 0, 80, '2024-01-01', '2024-12-31', 1, 'Bảo hiểm y tế cơ bản', 'INS001', NOW(), NOW(), 'System', 'System'),
(2, 'Bảo hiểm y tế cao cấp', 0, 90, '2024-01-01', '2024-12-31', 2, 'Bảo hiểm y tế cao cấp', 'INS002', NOW(), NOW(), 'System', 'System'),
(3, 'Bảo hiểm y tế gia đình', 0, 85, '2024-01-01', '2024-12-31', 3, 'Bảo hiểm y tế gia đình', 'INS003', NOW(), NOW(), 'System', 'System');

-- Insert sample prescriptions
INSERT INTO prescriptions (Id, Note, Status, PatientId, DoctorId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 'Uống thuốc đúng giờ, tái khám sau 1 tuần', 0, 1, 1, 'Đơn thuốc số 1', 'PRE001', NOW(), NOW(), 'System', 'System'),
(2, 'Uống thuốc đúng liều, tránh thức ăn cay nóng', 0, 2, 5, 'Đơn thuốc số 2', 'PRE002', NOW(), NOW(), 'System', 'System'),
(3, 'Uống thuốc trước bữa ăn 30 phút', 0, 3, 8, 'Đơn thuốc số 3', 'PRE003', NOW(), NOW(), 'System', 'System');

-- Insert sample prescription details
INSERT INTO prescriptiondetails (Id, Quantity, `Usage`, Status, PrescriptionId, MedicineId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 10, 'Uống 1 viên/lần, 2 lần/ngày', 0, 1, 1, 'Chi tiết đơn thuốc 1', 'PREDET001', NOW(), NOW(), 'System', 'System'),
(2, 7, 'Uống 1 viên/lần, 3 lần/ngày', 0, 2, 2, 'Chi tiết đơn thuốc 2', 'PREDET002', NOW(), NOW(), 'System', 'System'),
(3, 14, 'Uống 1 viên/lần, 1 lần/ngày', 0, 3, 3, 'Chi tiết đơn thuốc 3', 'PREDET003', NOW(), NOW(), 'System', 'System');







-- Insert sample payments
INSERT INTO payments (Id, PaymentDate, Amount, PaymentMethod, Payer, Notes, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, NOW(), 200000.00, 'Tiền mặt', 'David Miller', 'Thanh toán khám tổng quát', 'Thanh toán 1', 'PAY001', NOW(), NOW(), 'System', 'System'),
(2, NOW(), 270000.00, 'Chuyển khoản', 'Lisa Garcia', 'Thanh toán khám chuyên khoa', 'Thanh toán 2', 'PAY002', NOW(), NOW(), 'System', 'System'),
(3, NOW(), 400000.00, 'Thẻ tín dụng', 'James Rodriguez', 'Thanh toán siêu âm tim', 'Thanh toán 3', 'PAY003', NOW(), NOW(), 'System', 'System');













-- Insert sample invoices (before appointments)
INSERT INTO invoices (Id, InitialAmount, DiscountAmount, TotalAmount, Notes, Status, AppointmentId, PatientId, InsuranceId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 200000.00, 0.00, 200000.00, 'Khám tổng quát', 0, 1, 1, 1, 'Hóa đơn số 1', 'INV001', NOW(), NOW(), 'System', 'System'),
(2, 300000.00, 30000.00, 270000.00, 'Khám chuyên khoa', 0, 2, 2, 2, 'Hóa đơn số 2', 'INV002', NOW(), NOW(), 'System', 'System'),
(3, 400000.00, 0.00, 400000.00, 'Siêu âm tim', 0, 3, 3, 3, 'Hóa đơn số 3', 'INV003', NOW(), NOW(), 'System', 'System'),
(4, 200000.00, 0.00, 200000.00, 'Khám tổng quát mới', 0, 4, 1, 1, 'Hóa đơn số 4', 'INV004', NOW(), NOW(), 'System', 'System'),
(5, 300000.00, 30000.00, 270000.00, 'Khám chuyên khoa mới', 0, 5, 2, 2, 'Hóa đơn số 5', 'INV005', NOW(), NOW(), 'System', 'System'),
(6, 400000.00, 0.00, 400000.00, 'Siêu âm tim mới', 0, 6, 3, 3, 'Hóa đơn số 6', 'INV006', NOW(), NOW(), 'System', 'System');





-- Insert sample medical records (before appointments)
INSERT INTO medical_records (Id, Status, Diagnosis, TestResults, Notes, AppointmentId, PatientId, DoctorId, PrescriptionId, DiseaseId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 0, 'Cảm cúm nhẹ', 'Xét nghiệm máu bình thường', 'Nghỉ ngơi, uống nhiều nước', 1, 1, 1, 1, 1, 'Hồ sơ bệnh án 1', 'MEDREC001', NOW(), NOW(), 'System', 'System'),
(2, 0, 'Viêm phổi nhẹ', 'X-quang phổi có đốm mờ nhẹ', 'Uống thuốc đúng giờ', 2, 2, 5, 2, 2, 'Hồ sơ bệnh án 2', 'MEDREC002', NOW(), NOW(), 'System', 'System'),
(3, 0, 'Tăng huyết áp', 'Huyết áp 140/90 mmHg', 'Theo dõi huyết áp hàng ngày', 3, 3, 8, 3, 3, 'Hồ sơ bệnh án 3', 'MEDREC003', NOW(), NOW(), 'System', 'System'),
(4, 0, 'Khám tổng quát mới', 'Bình thường', 'Không có ghi chú', 4, 1, 1, 1, 1, 'Hồ sơ bệnh án 4', 'MEDREC004', NOW(), NOW(), 'System', 'System'),
(5, 0, 'Khám chuyên khoa mới', 'Bình thường', 'Không có ghi chú', 5, 2, 5, 2, 2, 'Hồ sơ bệnh án 5', 'MEDREC005', NOW(), NOW(), 'System', 'System'),
(6, 0, 'Siêu âm tim mới', 'Bình thường', 'Không có ghi chú', 6, 3, 8, 3, 3, 'Hồ sơ bệnh án 6', 'MEDREC006', NOW(), NOW(), 'System', 'System');

-- Insert sample appointments (after medical records are created)
-- Sử dụng thời gian từ hôm nay trở đi và cấu trúc mới với Shift
INSERT INTO appointments (Id, AppointmentDate, StartTime, EndTime, Shift, Status, Note, PatientId, ClinicId, ReceptionId, ServiceId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy, isSend) VALUES
(1, CURDATE(), NULL, NULL, 'morning', 0, 'Khám tổng quát', 1, 1, 1, 1, 'Lịch hẹn 1', 'APT001', NOW(), NOW(), 'System', 'System', 0),
(2, DATE_ADD(CURDATE(), INTERVAL 1 DAY), NULL, NULL, 'afternoon', 0, 'Khám chuyên khoa', 2, 2, 2, 2, 'Lịch hẹn 2', 'APT002', NOW(), NOW(), 'System', 'System', 0),
(3, DATE_ADD(CURDATE(), INTERVAL 2 DAY), NULL, NULL, 'morning', 0, 'Siêu âm tim', 3, 3, 1, 7, 'Lịch hẹn 3', 'APT003', NOW(), NOW(), 'System', 'System', 0),
(4, DATE_ADD(CURDATE(), INTERVAL 3 DAY), NULL, NULL, 'afternoon', 0, 'Khám tổng quát mới', 1, 1, 1, 1, 'Lịch hẹn 4', 'APT004', NOW(), NOW(), 'System', 'System', 0),
(5, DATE_ADD(CURDATE(), INTERVAL 4 DAY), NULL, NULL, 'morning', 0, 'Khám chuyên khoa mới', 2, 2, 2, 2, 'Lịch hẹn 5', 'APT005', NOW(), NOW(), 'System', 'System', 0),
(6, DATE_ADD(CURDATE(), INTERVAL 5 DAY), NULL, NULL, 'afternoon', 0, 'Siêu âm tim mới', 3, 3, 1, 7, 'Lịch hẹn 6', 'APT006', NOW(), NOW(), 'System', 'System', 0);

-- Insert sample doctor appointments (after appointments are created)
INSERT INTO doctor_appointment (Id, Status, DoctorId, AppointmentId) VALUES
(1, 0, 1, 1),
(2, 0, 5, 2),
(3, 0, 8, 3);

-- Insert sample nurse appointments (after appointments are created)
INSERT INTO nurse_appointments (Id, Status, NurseId, AppointmentId) VALUES
(1, 0, 1, 1),
(2, 0, 2, 2),
(3, 0, 3, 3);

-- Insert sample supplies (after appointments are created)
INSERT INTO supplies (Id, Status, Description, AppointmentId, UnitId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
(1, 0, 'Băng gạc y tế', 1, 3, 'Băng gạc', 'SUP001', NOW(), NOW(), 'System', 'System'),
(2, 0, 'Kim tiêm', 2, 1, 'Kim tiêm', 'SUP002', NOW(), NOW(), 'System', 'System'),
(3, 0, 'Găng tay y tế', 3, 3, 'Găng tay', 'SUP003', NOW(), NOW(), 'System', 'System');

-- Insert sample supply inventories (after supplies are created)
INSERT INTO supply_inventories (Id, Quantity, ImportDate, ExpiryDate, SupplierName, SupplyId) VALUES
(1, 100, NOW(), '2025-12-31', 'Công ty Thiết bị Y tế ABC', 1),
(2, 50, NOW(), '2025-06-30', 'Công ty Thiết bị Y tế XYZ', 2),
(3, 200, NOW(), '2025-12-31', 'Công ty Thiết bị Y tế ABC', 3);

-- Insert sample invoice details (after invoices are created)
INSERT INTO invoicedetail (Id, Discount, TotalAmount, Notes, InvoiceId, MedicineId, ServiceId, Name, Code, CreateDate, UpdateDate, CreateBy, UpdateBy, Status) VALUES
(1, 0.00, 200000.00, 'Khám tổng quát', 1, NULL, 1, 'Chi tiết hóa đơn 1', 'INVDET001', NOW(), NOW(), 'System', 'System', 0),
(2, 30000.00, 270000.00, 'Khám chuyên khoa', 2, NULL, 2, 'Chi tiết hóa đơn 2', 'INVDET002', NOW(), NOW(), 'System', 'System', 0),
(3, 0.00, 400000.00, 'Siêu âm tim', 3, NULL, 7, 'Chi tiết hóa đơn 3', 'INVDET003', NOW(), NOW(), 'System', 'System', 0);

-- Insert sample payment invoices (after invoices are created)
INSERT INTO payment_invoices (Id, AmountPaid, PaymentId, InvoiceId) VALUES
(1, 200000.00, 1, 1),
(2, 270000.00, 2, 2),
(3, 400000.00, 3, 3);

-- Insert sample feedbacks (after appointments are created)
INSERT INTO feedbacks (Id, Content, CreateDate, PatientId, DoctorId, AppointmentId) VALUES
(1, 'Bác sĩ rất tận tâm và chuyên nghiệp', NOW(), 1, 1, 1),
(2, 'Dịch vụ khám bệnh rất tốt', NOW(), 2, 5, 2),
(3, 'Nhân viên y tế rất thân thiện', NOW(), 3, 8, 3);

-- Insert doctor shifts for all doctors - Sử dụng thời gian từ hôm nay trở đi
-- Mỗi bác sĩ có ca sáng (08:00-12:00) và chiều (13:00-17:00) liên tục từ hôm nay

DELETE FROM doctor_shifts;
ALTER TABLE doctor_shifts AUTO_INCREMENT = 1;

-- Tạo ca làm việc cho 30 ngày tiếp theo từ hôm nay
INSERT INTO doctor_shifts (DoctorId, ShiftDate, ShiftType, StartTime, EndTime, Notes, CreateDate, UpdateDate, CreateBy, UpdateBy) VALUES
-- Ngày 1 (hôm nay)
(1, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(1, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 0 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(CURDATE(), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),

-- Ngày 2
(1, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(1, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 1 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 1 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),

-- Ngày 3
(1, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(1, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 2 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 2 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),

-- Ngày 4-30 (tạo thêm 27 ngày nữa)
(1, DATE_ADD(CURDATE(), INTERVAL 3 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 3 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(1, DATE_ADD(CURDATE(), INTERVAL 3 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 3 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 4 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 4 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 4 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 4 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 5 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 5 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 5 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 5 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 6 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 6 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 6 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 6 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 7 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 7 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 7 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 7 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 8 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 8 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 8 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 8 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 9 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 9 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 9 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 9 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 10 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 10 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 10 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 10 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 11 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 11 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 11 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 11 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),

-- Tiếp tục cho các ngày còn lại (12-30)
(1, DATE_ADD(CURDATE(), INTERVAL 12 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 12 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(1, DATE_ADD(CURDATE(), INTERVAL 12 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 12 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 13 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 13 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 13 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 13 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 14 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 14 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 14 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 14 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 15 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 15 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 16 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 16 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 16 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 16 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 17 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 17 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 17 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 17 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 18 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 18 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 18 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 18 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 19 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 19 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 19 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 19 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 20 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 20 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 20 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 20 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),

-- Ngày 21-30
(1, DATE_ADD(CURDATE(), INTERVAL 21 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 21 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(1, DATE_ADD(CURDATE(), INTERVAL 21 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 21 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 22 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 22 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(2, DATE_ADD(CURDATE(), INTERVAL 22 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 22 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 23 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 23 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(3, DATE_ADD(CURDATE(), INTERVAL 23 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 23 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 24 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 24 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(4, DATE_ADD(CURDATE(), INTERVAL 24 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 24 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 25 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 25 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(5, DATE_ADD(CURDATE(), INTERVAL 25 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 25 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 26 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 26 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(6, DATE_ADD(CURDATE(), INTERVAL 26 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 26 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 27 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 27 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(7, DATE_ADD(CURDATE(), INTERVAL 27 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 27 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 28 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 28 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(8, DATE_ADD(CURDATE(), INTERVAL 28 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 28 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 29 DAY), 'Morning', '08:00:00', '12:00:00', CONCAT('Ca sáng ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 29 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System'),
(9, DATE_ADD(CURDATE(), INTERVAL 29 DAY), 'Afternoon', '13:00:00', '17:00:00', CONCAT('Ca chiều ngày ', DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 29 DAY), '%d/%m/%Y')), NOW(), NOW(), 'System', 'System');
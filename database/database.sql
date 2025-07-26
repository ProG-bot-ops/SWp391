-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: localhost    Database: hospitalmanagement
-- ------------------------------------------------------
-- Server version	8.0.42

-- Create database if it doesn't exist
CREATE DATABASE IF NOT EXISTS `hospitalmanagement`;

-- Select the database to use
USE `hospitalmanagement`;

SET SQL_SAFE_UPDATES = 0;

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appointments`
--

DROP TABLE IF EXISTS `appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appointments` (
  `Id` int NOT NULL,
  `AppointmentDate` date NOT NULL,
  `StartTime` time NULL COMMENT 'Thời gian bắt đầu khám (được set khi status = InProgress)',
  `EndTime` time NULL COMMENT 'Thời gian kết thúc khám (được set khi status = Completed)',
  `Shift` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'morning hoặc afternoon',
  `Status` int NOT NULL,
  `Note` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PatientId` int NOT NULL,
  `ClinicId` int NOT NULL,
  `ReceptionId` int DEFAULT NULL,
  `ServiceId` int DEFAULT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `isSend` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `IX_Appointments_ClinicId` (`ClinicId`),
  KEY `IX_Appointments_PatientId` (`PatientId`),
  KEY `IX_Appointments_ReceptionId` (`ReceptionId`),
  KEY `IX_Appointments_ServiceId` (`ServiceId`),
  KEY `IX_Appointments_Shift` (`Shift`),
  KEY `IX_Appointments_Date_Shift` (`AppointmentDate`, `Shift`),
  CONSTRAINT `FK_Appointments_Clinics_ClinicId` FOREIGN KEY (`ClinicId`) REFERENCES `clinics` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_Invoices_Id` FOREIGN KEY (`Id`) REFERENCES `invoices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `patients` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_Receptions_ReceptionId` FOREIGN KEY (`ReceptionId`) REFERENCES `receptions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_Services_ServiceId` FOREIGN KEY (`ServiceId`) REFERENCES `services` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appointments`
--

LOCK TABLES `appointments` WRITE;
/*!40000 ALTER TABLE `appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clinics`
--

DROP TABLE IF EXISTS `clinics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clinics` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Status` int NOT NULL,
  `Type` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ImageUrl` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clinics`
--

LOCK TABLES `clinics` WRITE;
/*!40000 ALTER TABLE `clinics` DISABLE KEYS */;
/*!40000 ALTER TABLE `clinics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `departments`
--

DROP TABLE IF EXISTS `departments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `departments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Description` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TotalAmountOfPeople` int NOT NULL,
  `Status` int NOT NULL,
  `ClinicId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Departments_ClinicId` (`ClinicId`),
  CONSTRAINT `FK_Departments_Clinics_ClinicId` FOREIGN KEY (`ClinicId`) REFERENCES `clinics` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `departments`
--

LOCK TABLES `departments` WRITE;
/*!40000 ALTER TABLE `departments` DISABLE KEYS */;
/*!40000 ALTER TABLE `departments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `diseasedetail`
--

DROP TABLE IF EXISTS `diseasedetail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `diseasedetail` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Description` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `DiseaseId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DiseaseDetail_DiseaseId` (`DiseaseId`),
  CONSTRAINT `FK_DiseaseDetail_Diseases_DiseaseId` FOREIGN KEY (`DiseaseId`) REFERENCES `diseases` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `diseasedetail`
--

LOCK TABLES `diseasedetail` WRITE;
/*!40000 ALTER TABLE `diseasedetail` DISABLE KEYS */;
/*!40000 ALTER TABLE `diseasedetail` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `diseases`
--

DROP TABLE IF EXISTS `diseases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `diseases` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Description` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `diseases`
--

LOCK TABLES `diseases` WRITE;
/*!40000 ALTER TABLE `diseases` DISABLE KEYS */;
/*!40000 ALTER TABLE `diseases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doctor_appointment`
--

DROP TABLE IF EXISTS `doctor_appointment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doctor_appointment` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Status` int NOT NULL,
  `DoctorId` int NOT NULL,
  `AppointmentId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Doctor_Appointment_AppointmentId` (`AppointmentId`),
  KEY `IX_Doctor_Appointment_DoctorId` (`DoctorId`),
  CONSTRAINT `FK_Doctor_Appointment_Appointments_AppointmentId` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Doctor_Appointment_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `doctors` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doctor_appointment`
--

LOCK TABLES `doctor_appointment` WRITE;
/*!40000 ALTER TABLE `doctor_appointment` DISABLE KEYS */;
/*!40000 ALTER TABLE `doctor_appointment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doctor_shifts`
--

DROP TABLE IF EXISTS `doctor_shifts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doctor_shifts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DoctorId` int NOT NULL,
  `ShiftDate` datetime(6) NOT NULL,
  `ShiftType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `StartTime` time(6) NOT NULL,
  `EndTime` time(6) NOT NULL,
  `Notes` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreateDate` datetime(6) DEFAULT NULL,
  `UpdateDate` datetime(6) DEFAULT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_doctor_shifts_DoctorId` (`DoctorId`),
  CONSTRAINT `FK_doctor_shifts_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `doctors` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doctor_shifts`
--

LOCK TABLES `doctor_shifts` WRITE;
/*!40000 ALTER TABLE `doctor_shifts` DISABLE KEYS */;
/*!40000 ALTER TABLE `doctor_shifts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shift_request`
--

DROP TABLE IF EXISTS `shift_request`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shift_request` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DoctorId` int NOT NULL,
  `ShiftId` int NOT NULL,
  `RequestType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Reason` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ApprovedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ShiftRequest_DoctorId` (`DoctorId`),
  KEY `IX_ShiftRequest_ShiftId` (`ShiftId`),
  CONSTRAINT `FK_ShiftRequest_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `doctors` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShiftRequest_DoctorShifts_ShiftId` FOREIGN KEY (`ShiftId`) REFERENCES `doctor_shifts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shift_request`
--

LOCK TABLES `shift_request` WRITE;
/*!40000 ALTER TABLE `shift_request` DISABLE KEYS */;
/*!40000 ALTER TABLE `shift_request` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doctors`
--

DROP TABLE IF EXISTS `doctors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doctors` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Gender` int NOT NULL,
  `Dob` datetime(6) NOT NULL,
  `CCCD` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ImageURL` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LicenseNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `YearOfExperience` float NOT NULL,
  `WorkingHours` float NOT NULL,
  `Status` int NOT NULL,
  `UserId` int NOT NULL,
  `DepartmentId` int NOT NULL,
  `ClinicId` int DEFAULT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Doctors_DepartmentId` (`DepartmentId`),
  KEY `IX_Doctors_UserId` (`UserId`),
  KEY `IX_Doctors_ClinicId` (`ClinicId`),
  CONSTRAINT `FK_Doctors_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Doctors_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Doctors_Clinics_ClinicId` FOREIGN KEY (`ClinicId`) REFERENCES `clinics` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doctors`
--

LOCK TABLES `doctors` WRITE;
/*!40000 ALTER TABLE `doctors` DISABLE KEYS */;
/*!40000 ALTER TABLE `doctors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `feedbacks`
--

DROP TABLE IF EXISTS `feedbacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `feedbacks` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Content` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `PatientId` int NOT NULL,
  `DoctorId` int DEFAULT NULL,
  `AppointmentId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Feedbacks_AppointmentId` (`AppointmentId`),
  KEY `IX_Feedbacks_DoctorId` (`DoctorId`),
  KEY `IX_Feedbacks_PatientId` (`PatientId`),
  CONSTRAINT `FK_Feedbacks_Appointments_AppointmentId` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Feedbacks_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `doctors` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Feedbacks_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `patients` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `feedbacks`
--

LOCK TABLES `feedbacks` WRITE;
/*!40000 ALTER TABLE `feedbacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `feedbacks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `insurances`
--

DROP TABLE IF EXISTS `insurances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `insurances` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Description` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `CoveragePercent` int NOT NULL,
  `StartDate` datetime(6) NOT NULL,
  `EndDate` datetime(6) NOT NULL,
  `PatientId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Insurances_PatientId` (`PatientId`),
  CONSTRAINT `FK_Insurances_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `patients` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `insurances`
--

LOCK TABLES `insurances` WRITE;
/*!40000 ALTER TABLE `insurances` DISABLE KEYS */;
/*!40000 ALTER TABLE `insurances` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `invoicedetail`
--

DROP TABLE IF EXISTS `invoicedetail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `invoicedetail` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Status` int NOT NULL,
  `Discount` decimal(65,30) NOT NULL,
  `TotalAmount` decimal(65,30) NOT NULL,
  `Notes` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `InvoiceId` int NOT NULL,
  `MedicineId` int DEFAULT NULL,
  `ServiceId` int DEFAULT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_InvoiceDetail_InvoiceId` (`InvoiceId`),
  KEY `IX_InvoiceDetail_MedicineId` (`MedicineId`),
  KEY `IX_InvoiceDetail_ServiceId` (`ServiceId`),
  CONSTRAINT `FK_InvoiceDetail_Invoices_InvoiceId` FOREIGN KEY (`InvoiceId`) REFERENCES `invoices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_InvoiceDetail_Medicines_MedicineId` FOREIGN KEY (`MedicineId`) REFERENCES `medicines` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_InvoiceDetail_Services_ServiceId` FOREIGN KEY (`ServiceId`) REFERENCES `services` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoicedetail`
--

LOCK TABLES `invoicedetail` WRITE;
/*!40000 ALTER TABLE `invoicedetail` DISABLE KEYS */;
/*!40000 ALTER TABLE `invoicedetail` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `invoices`
--

DROP TABLE IF EXISTS `invoices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `invoices` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `InitialAmount` decimal(65,30) NOT NULL,
  `DiscountAmount` decimal(65,30) NOT NULL,
  `TotalAmount` decimal(65,30) NOT NULL,
  `Notes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `AppointmentId` int NOT NULL,
  `InsuranceId` int DEFAULT NULL,
  `PatientId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Invoices_InsuranceId` (`InsuranceId`),
  KEY `IX_Invoices_PatientId` (`PatientId`),
  CONSTRAINT `FK_Invoices_Insurances_InsuranceId` FOREIGN KEY (`InsuranceId`) REFERENCES `insurances` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Invoices_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `patients` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoices`
--

LOCK TABLES `invoices` WRITE;
/*!40000 ALTER TABLE `invoices` DISABLE KEYS */;
/*!40000 ALTER TABLE `invoices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medical_records`
--

DROP TABLE IF EXISTS `medical_records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medical_records` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Status` int NOT NULL,
  `Diagnosis` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TestResults` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Notes` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `AppointmentId` int NOT NULL,
  `PatientId` int NOT NULL,
  `DoctorId` int NOT NULL,
  `PrescriptionId` int DEFAULT NULL,
  `DiseaseId` int DEFAULT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Medical_Records_DiseaseId` (`DiseaseId`),
  KEY `IX_Medical_Records_DoctorId` (`DoctorId`),
  KEY `IX_Medical_Records_PatientId` (`PatientId`),
  KEY `IX_Medical_Records_PrescriptionId` (`PrescriptionId`),
  CONSTRAINT `FK_Medical_Records_Diseases_DiseaseId` FOREIGN KEY (`DiseaseId`) REFERENCES `diseases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Medical_Records_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `doctors` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Medical_Records_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `patients` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Medical_Records_Prescriptions_PrescriptionId` FOREIGN KEY (`PrescriptionId`) REFERENCES `prescriptions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medical_records`
--

LOCK TABLES `medical_records` WRITE;
/*!40000 ALTER TABLE `medical_records` DISABLE KEYS */;
/*!40000 ALTER TABLE `medical_records` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicine_detail`
--

DROP TABLE IF EXISTS `medicine_detail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicine_detail` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MedicineId` int NOT NULL,
  `Ingredients` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ExpiryDate` datetime(6) DEFAULT NULL,
  `Manufacturer` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Warning` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `StorageInstructions` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Status` int NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_medicine_detail_MedicineId` (`MedicineId`),
  CONSTRAINT `FK_medicine_detail_Medicines_MedicineId` FOREIGN KEY (`MedicineId`) REFERENCES `medicines` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicine_detail`
--

LOCK TABLES `medicine_detail` WRITE;
/*!40000 ALTER TABLE `medicine_detail` DISABLE KEYS */;
/*!40000 ALTER TABLE `medicine_detail` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicine_import_details`
--

DROP TABLE IF EXISTS `medicine_import_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicine_import_details` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ImportId` int NOT NULL,
  `MedicineId` int NOT NULL,
  `BatchNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Quantity` int NOT NULL,
  `UnitPrice` decimal(65,30) NOT NULL,
  `ManufactureDate` datetime(6) NOT NULL,
  `ExpiryDate` datetime(6) NOT NULL,
  `UnitId` int NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) DEFAULT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_medicine_import_details_MedicineId_BatchNumber` (`MedicineId`,`BatchNumber`),
  KEY `IX_medicine_import_details_ImportId` (`ImportId`),
  KEY `IX_medicine_import_details_UnitId` (`UnitId`),
  CONSTRAINT `FK_medicine_import_details_medicine_imports_ImportId` FOREIGN KEY (`ImportId`) REFERENCES `medicine_imports` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_medicine_import_details_Medicines_MedicineId` FOREIGN KEY (`MedicineId`) REFERENCES `medicines` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_medicine_import_details_Units_UnitId` FOREIGN KEY (`UnitId`) REFERENCES `units` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicine_import_details`
--

LOCK TABLES `medicine_import_details` WRITE;
/*!40000 ALTER TABLE `medicine_import_details` DISABLE KEYS */;
/*!40000 ALTER TABLE `medicine_import_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicine_imports`
--

DROP TABLE IF EXISTS `medicine_imports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicine_imports` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Notes` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SupplierId` int NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) DEFAULT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_medicine_imports_SupplierId` (`SupplierId`),
  CONSTRAINT `FK_medicine_imports_suppliers_SupplierId` FOREIGN KEY (`SupplierId`) REFERENCES `suppliers` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicine_imports`
--

LOCK TABLES `medicine_imports` WRITE;
/*!40000 ALTER TABLE `medicine_imports` DISABLE KEYS */;
/*!40000 ALTER TABLE `medicine_imports` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicine_inventories`
--

DROP TABLE IF EXISTS `medicine_inventories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicine_inventories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Quantity` int NOT NULL,
  `BatchNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UnitPrice` decimal(65,30) NOT NULL,
  `ImportDate` datetime(6) NOT NULL,
  `ExpiryDate` datetime(6) NOT NULL,
  `Status` int NOT NULL,
  `MedicineId` int NOT NULL,
  `ImportDetailId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Medicine_Inventories_ImportDetailId` (`ImportDetailId`),
  KEY `IX_Medicine_Inventories_MedicineId` (`MedicineId`),
  CONSTRAINT `FK_Medicine_Inventories_medicine_import_details_ImportDetailId` FOREIGN KEY (`ImportDetailId`) REFERENCES `medicine_import_details` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Medicine_Inventories_Medicines_MedicineId` FOREIGN KEY (`MedicineId`) REFERENCES `medicines` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicine_inventories`
--

LOCK TABLES `medicine_inventories` WRITE;
/*!40000 ALTER TABLE `medicine_inventories` DISABLE KEYS */;
/*!40000 ALTER TABLE `medicine_inventories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicinecategories`
--

DROP TABLE IF EXISTS `medicinecategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicinecategories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Description` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ImageUrl` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) DEFAULT NULL,
  `CreateBy` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicinecategories`
--

LOCK TABLES `medicinecategories` WRITE;
/*!40000 ALTER TABLE `medicinecategories` DISABLE KEYS */;
/*!40000 ALTER TABLE `medicinecategories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicines`
--

DROP TABLE IF EXISTS `medicines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicines` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ImageUrl` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` int NOT NULL,
  `Description` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UnitId` int NOT NULL,
  `Prescribed` int NOT NULL,
  `Dosage` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MedicineCategoryId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Medicines_MedicineCategoryId` (`MedicineCategoryId`),
  KEY `IX_Medicines_UnitId` (`UnitId`),
  CONSTRAINT `FK_Medicines_MedicineCategories_MedicineCategoryId` FOREIGN KEY (`MedicineCategoryId`) REFERENCES `medicinecategories` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Medicines_Units_UnitId` FOREIGN KEY (`UnitId`) REFERENCES `units` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicines`
--

LOCK TABLES `medicines` WRITE;
/*!40000 ALTER TABLE `medicines` DISABLE KEYS */;
/*!40000 ALTER TABLE `medicines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notifications`
--

DROP TABLE IF EXISTS `notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifications` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Content` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `SendTime` datetime(6) NOT NULL,
  `UserId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Notifications_UserId` (`UserId`),
  CONSTRAINT `FK_Notifications_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifications`
--

LOCK TABLES `notifications` WRITE;
/*!40000 ALTER TABLE `notifications` DISABLE KEYS */;
/*!40000 ALTER TABLE `notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nurse_appointments`
--

DROP TABLE IF EXISTS `nurse_appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nurse_appointments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Status` int NOT NULL,
  `NurseId` int NOT NULL,
  `AppointmentId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Nurse_Appointments_AppointmentId` (`AppointmentId`),
  KEY `IX_Nurse_Appointments_NurseId` (`NurseId`),
  CONSTRAINT `FK_Nurse_Appointments_Appointments_AppointmentId` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Nurse_Appointments_Nurses_NurseId` FOREIGN KEY (`NurseId`) REFERENCES `nurses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nurse_appointments`
--

LOCK TABLES `nurse_appointments` WRITE;
/*!40000 ALTER TABLE `nurse_appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `nurse_appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nurses`
--

DROP TABLE IF EXISTS `nurses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nurses` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Gender` int NOT NULL,
  `Dob` datetime(6) NOT NULL,
  `CCCD` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ImageURL` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `UserId` int NOT NULL,
  `DepartmentId` int NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Nurses_DepartmentId` (`DepartmentId`),
  KEY `IX_Nurses_UserId` (`UserId`),
  CONSTRAINT `FK_Nurses_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Nurses_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nurses`
--

LOCK TABLES `nurses` WRITE;
/*!40000 ALTER TABLE `nurses` DISABLE KEYS */;
/*!40000 ALTER TABLE `nurses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patients`
--

DROP TABLE IF EXISTS `patients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patients` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Gender` int NOT NULL,
  `Dob` datetime(6) NOT NULL,
  `CCCD` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EmergencyContact` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Address` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `insuranceNumber` varchar(100) DEFAULT NULL,
  `Allergies` varchar(100) DEFAULT NULL,
  `Status` int NOT NULL,
  `BloodType` varchar(100) DEFAULT NULL,
  `ImageURL` varchar(100) DEFAULT NULL,
  `UserId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Patients_UserId` (`UserId`),
  CONSTRAINT `FK_Patients_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patients`
--

LOCK TABLES `patients` WRITE;
/*!40000 ALTER TABLE `patients` DISABLE KEYS */;
/*!40000 ALTER TABLE `patients` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payment_invoices`
--

DROP TABLE IF EXISTS `payment_invoices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_invoices` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `AmountPaid` decimal(65,30) NOT NULL,
  `PaymentId` int NOT NULL,
  `InvoiceId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Payment_Invoices_InvoiceId` (`InvoiceId`),
  KEY `IX_Payment_Invoices_PaymentId` (`PaymentId`),
  CONSTRAINT `FK_Payment_Invoices_Invoices_InvoiceId` FOREIGN KEY (`InvoiceId`) REFERENCES `invoices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Payment_Invoices_Payments_PaymentId` FOREIGN KEY (`PaymentId`) REFERENCES `payments` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_invoices`
--

LOCK TABLES `payment_invoices` WRITE;
/*!40000 ALTER TABLE `payment_invoices` DISABLE KEYS */;
/*!40000 ALTER TABLE `payment_invoices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PaymentDate` datetime(6) NOT NULL,
  `Amount` decimal(65,30) NOT NULL,
  `PaymentMethod` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Payer` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Notes` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prescriptiondetails`
--

DROP TABLE IF EXISTS `prescriptiondetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prescriptiondetails` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Quantity` int NOT NULL,
  `Usage` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `PrescriptionId` int NOT NULL,
  `MedicineId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PrescriptionDetails_MedicineId` (`MedicineId`),
  KEY `IX_PrescriptionDetails_PrescriptionId` (`PrescriptionId`),
  CONSTRAINT `FK_PrescriptionDetails_Medicines_MedicineId` FOREIGN KEY (`MedicineId`) REFERENCES `medicines` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PrescriptionDetails_Prescriptions_PrescriptionId` FOREIGN KEY (`PrescriptionId`) REFERENCES `prescriptions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prescriptiondetails`
--

LOCK TABLES `prescriptiondetails` WRITE;
/*!40000 ALTER TABLE `prescriptiondetails` DISABLE KEYS */;
/*!40000 ALTER TABLE `prescriptiondetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prescriptions`
--

DROP TABLE IF EXISTS `prescriptions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prescriptions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Note` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `PatientId` int NOT NULL,
  `DoctorId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Prescriptions_DoctorId` (`DoctorId`),
  KEY `IX_Prescriptions_PatientId` (`PatientId`),
  CONSTRAINT `FK_Prescriptions_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `doctors` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Prescriptions_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `patients` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prescriptions`
--

LOCK TABLES `prescriptions` WRITE;
/*!40000 ALTER TABLE `prescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `prescriptions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `receptions`
--

DROP TABLE IF EXISTS `receptions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `receptions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Gender` int NOT NULL,
  `Dob` datetime(6) NOT NULL,
  `CCCD` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ImageURL` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `UserId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Receptions_UserId` (`UserId`),
  CONSTRAINT `FK_Receptions_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `receptions`
--

LOCK TABLES `receptions` WRITE;
/*!40000 ALTER TABLE `receptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `receptions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `services`
--

DROP TABLE IF EXISTS `services`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `services` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ImageUrl` varchar(255) DEFAULT NULL,
  `Price` decimal(18,2) NOT NULL,
  `Status` int NOT NULL,
  `DepartmentId` int DEFAULT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_services_Code` (`Code`),
  KEY `IX_services_DepartmentId` (`DepartmentId`),
  CONSTRAINT `FK_services_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `services`
--

LOCK TABLES `services` WRITE;
/*!40000 ALTER TABLE `services` DISABLE KEYS */;
/*!40000 ALTER TABLE `services` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `suppliers`
--

DROP TABLE IF EXISTS `suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `suppliers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Address` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) DEFAULT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `suppliers`
--

LOCK TABLES `suppliers` WRITE;
/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplies`
--

DROP TABLE IF EXISTS `supplies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplies` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Status` int NOT NULL,
  `Description` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `AppointmentId` int NOT NULL,
  `UnitId` int DEFAULT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreateDate` datetime(6) NOT NULL,
  `UpdateDate` datetime(6) NOT NULL,
  `CreateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UpdateBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Supplies_AppointmentId` (`AppointmentId`),
  KEY `IX_Supplies_UnitId` (`UnitId`),
  CONSTRAINT `FK_Supplies_Appointments_AppointmentId` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Supplies_Units_UnitId` FOREIGN KEY (`UnitId`) REFERENCES `units` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplies`
--

LOCK TABLES `supplies` WRITE;
/*!40000 ALTER TABLE `supplies` DISABLE KEYS */;
/*!40000 ALTER TABLE `supplies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supply_inventories`
--

DROP TABLE IF EXISTS `supply_inventories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supply_inventories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Quantity` int NOT NULL,
  `ImportDate` datetime(6) NOT NULL,
  `ExpiryDate` datetime(6) NOT NULL,
  `SupplierName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SupplyId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Supply_Inventories_SupplyId` (`SupplyId`),
  CONSTRAINT `FK_Supply_Inventories_Supplies_SupplyId` FOREIGN KEY (`SupplyId`) REFERENCES `supplies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supply_inventories`
--

LOCK TABLES `supply_inventories` WRITE;
/*!40000 ALTER TABLE `supply_inventories` DISABLE KEYS */;
/*!40000 ALTER TABLE `supply_inventories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `units`
--

DROP TABLE IF EXISTS `units`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `units` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `units`
--

LOCK TABLES `units` WRITE;
/*!40000 ALTER TABLE `units` DISABLE KEYS */;
/*!40000 ALTER TABLE `units` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_roles`
--

DROP TABLE IF EXISTS `user_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_roles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` int NOT NULL,
  `UserId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_User_Roles_RoleId` (`RoleId`),
  KEY `IX_User_Roles_UserId` (`UserId`),
  CONSTRAINT `FK_User_Roles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_User_Roles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_roles`
--

LOCK TABLES `user_roles` WRITE;
/*!40000 ALTER TABLE `user_roles` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Password` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `RefreshToken` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `RefreshTokenExpiryTime` datetime(6) DEFAULT NULL,
  `ResetPasswordToken` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ResetPasswordTokenExpiryTime` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;



/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-07-05 14:36:37

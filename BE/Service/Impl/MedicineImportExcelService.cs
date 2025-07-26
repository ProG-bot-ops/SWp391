using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ImportMedicineEX;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;
using SWP391_SE1914_ManageHospital.Ultility.Validation;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class MedicineImportExcelService : IMedicineImportExcelService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicineImportExcelMapper _mapper;

        public MedicineImportExcelService(ApplicationDBContext context, IMedicineImportExcelMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MedicineImportRequest> ParseImportExcelToRequest(IFormFile file, int supplierId)
        {
            var request = new MedicineImportRequest
            {
                ImportCode = GenerateCode.GenerateMedicineImportCode(),
                ImportName = "Import từ Excel",
                SupplierId = supplierId,
                Details = new List<MedicineImportDetailRequest>()
            };

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rowCount = worksheet.LastRowUsed().RowNumber();

            for (int row = 2; row <= rowCount; row++)
            {
                bool isEmptyRow = true;
                for (int col = 1; col <= 13; col++)
                {
                    if (!string.IsNullOrWhiteSpace(worksheet.Cell(row, col).GetString()))
                    {
                        isEmptyRow = false;
                        break;
                    }
                }

                if (isEmptyRow)
                    continue;
                string prescribedCell = worksheet.Cell(row, 9).GetString().Trim().ToUpper();
                PrescribedMedication prescribedEnum;

                if (prescribedCell == "ETC")
                {
                    prescribedEnum = PrescribedMedication.Yes;
                }
                else if (prescribedCell == "OTC")
                {
                    prescribedEnum = PrescribedMedication.No;
                }
                else
                {
                    throw new Exception($"Dòng {row}: Cột Prescribed không hợp lệ.");
                }


                var detail = new MedicineImportDetailRequest
                {
                    MedicineCode = worksheet.Cell(row, 1).GetString(),
                    MedicineName = worksheet.Cell(row, 2).GetString(),
                    UnitName = worksheet.Cell(row, 3).GetString(),
                    BatchNumber = worksheet.Cell(row, 4).GetString(),
                    Quantity = int.TryParse(worksheet.Cell(row, 5).GetString(), out int q) ? q : 0,
                    UnitPrice = decimal.TryParse(worksheet.Cell(row, 6).GetString(), out decimal p) ? p : 0,
                    ManufactureDate = DateTime.TryParse(worksheet.Cell(row, 7).GetString(), out var mfg) ? mfg : DateTime.MinValue,
                    ExpiryDate = DateTime.TryParse(worksheet.Cell(row, 8).GetString(), out var exp) ? exp : DateTime.MinValue,
                    Prescribed = prescribedEnum,
                    Dosage = worksheet.Cell(row, 10).GetString(),
                    Ingredients = worksheet.Cell(row, 11).GetString(),
                    CategoryName = worksheet.Cell(row, 12).GetString(),
                    StorageInstructions = worksheet.Cell(row, 13).GetString(),
                    MedicineDescription = worksheet.Cell(row, 14).GetString(),
                    MedicineDetailDescription = worksheet.Cell(row, 15).GetString(),
                    Waring = worksheet.Cell(row, 16).GetString(),
                };
                MedicineImportExcelValidation.ValidateRow(detail, row);
                request.Details.Add(detail);
            }

            return request;
        }

        public async Task<bool> ConfirmImportAsync(MedicineImportRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == request.SupplierId);
                if (supplier == null) return false;

                var import = _mapper.MapToImportEntity(request, supplier.Id);
                _context.MedicineImports.Add(import);
                await _context.SaveChangesAsync();

                foreach (var detail in request.Details)
                {
                    var category = await _context.MedicineCategories
                        .FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == detail.CategoryName.ToLower().Trim());

                    if (category == null)
                    {
                        category = new MedicineCategory
                        {
                            Name = detail.CategoryName,
                            Code = GenerateCode.GenerateMedicineCategoryCode(),
                            ImageUrl = "Chưa có ảnh",
                            Description = "Chưa thêm",
                            Status = MedicineCategoryStatus.InStock,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                            CreateBy = "system",
                            UpdateBy = "system"
                        };
                        _context.MedicineCategories.Add(category);
                        await _context.SaveChangesAsync();
                    }

                    var unit = await _context.Units.FirstOrDefaultAsync(u => u.Name == detail.UnitName);
                    if (unit == null)
                    {
                        unit = new Unit
                        {
                            Name = detail.UnitName,
                            Status = 1
                        };
                        _context.Units.Add(unit);
                        await _context.SaveChangesAsync();
                    }

                    var medicine = await _context.Medicines.FirstOrDefaultAsync(m => m.Code == detail.MedicineCode);

                   

                    if (medicine == null)
                    {
                        medicine = new Medicine
                        {
                            Name = detail.MedicineName,
                            Code = detail.MedicineCode,
                            ImageUrl = "Chưa có ảnh",
                            UnitId = unit.Id,
                            UnitPrice = detail.UnitPrice,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                            CreateBy = "system",
                            UpdateBy = "system",
                            Status = MedicineStatus.Active,
                            Description = detail.MedicineDescription,
                            Dosage = detail.Dosage,
                            Prescribed = detail.Prescribed,
                            MedicineCategoryId = category.Id,
                        };
                        _context.Medicines.Add(medicine);
                        await _context.SaveChangesAsync();

                        var medicineDetail = new MedicineDetail
                        {
                            MedicineId = medicine.Id,
                            Ingredients = detail.Ingredients,
                            ExpiryDate = detail.ExpiryDate,
                            Manufacturer = detail.ManufactureDate,
                            StorageInstructions = detail.StorageInstructions,
                            Warning = detail.Waring,
                            Description = detail.MedicineDetailDescription,
                            Status = 1,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                            CreateBy = "system",
                            UpdateBy = "system"
                        };
                        _context.MedicineDetails.Add(medicineDetail);
                        await _context.SaveChangesAsync();
                    }
                    var existingBatch = await _context.MedicineImportDetails
                               .FirstOrDefaultAsync(m => m.MedicineId == medicine.Id &&
                               m.BatchNumber == detail.BatchNumber &&
                               m.SupplierId == request.SupplierId);

                    if (existingBatch != null)
                    {
                        throw new Exception($"Lỗi: Số lô {detail.BatchNumber} của thuốc {detail.MedicineName} đã tồn tại trong lần nhập trước của nhà cung cấp này.");
                    }


                    var importDetail = _mapper.MapToImportDetailEntity(detail, medicine.Id, import.Id);
                    importDetail.UnitId = unit.Id;
                    importDetail.SupplierId = supplier.Id;
                    importDetail.Code = GenerateCode.GenerateMedicineImportDetailCode();
                    _context.MedicineImportDetails.Add(importDetail);
                    await _context.SaveChangesAsync();

                    var inventory = _mapper.MapToInventoryEntity(detail, medicine.Id, importDetail.Id);
                    _context.Medicine_Inventories.Add(inventory);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Xác nhận nhập kho thất bại: {ex.Message}", ex);
            }
        }
    }
}
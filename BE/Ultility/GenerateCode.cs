namespace SWP391_SE1914_ManageHospital.Ultility;

public class GenerateCode
{
    private static readonly Random random = new Random();
    public static string GenerateDepartmentCode()
    {
        
        int deRand = random.Next(100000, 999999);
        return "DE" + deRand;
    }
    public static string GenerateClinicCode()
    {
        
        int cliRand = random.Next(100000, 999999);
        return "C" + cliRand;
    }
    public static string GenerateDiseaseCode()
    {
        
        int diRand = random.Next(100000, 999999);
        return "DI" + diRand;
    }
    public static string GenerateDiseaseDetailCode()
    {

        int didRand = random.Next(100000, 999999);
        return "DID" + didRand;
    }
    public static string GenerateDoctorCode()
    {

        int drRand = random.Next(100000, 999999);
        return "DR" + drRand;
    }
    public static string GenerateInvoiceCode()
    {

        int invRand = random.Next(100000, 999999);
        return "INV" + invRand;
    }
    public static string GenerateInvoiceDetailCode()
    {

        int invdRand = random.Next(100000, 999999);
        return "ORD" + invdRand;
    }
    public static string GenerateMedicalRecordCode()
    {

        int mrRand = random.Next(100000, 999999);
        return "MR" + mrRand;
    }
    public static string GenerateMedicineCode()
    {

        int medRand = random.Next(100000, 999999);
        return "MED" + medRand;
    }

    public static string GenerateMedicineCategoryCode()
    {

        int medcatRand = random.Next(100000, 999999);
        return "MEDC" + medcatRand;
    }
    public static string GenerateMedicineImportDetailCode()
    {

        int medcatRand = random.Next(100000, 999999);
        return "MID" + medcatRand;
    }
    public static string GenerateMedicineImportCode()
    {

        int medcatRand = random.Next(100000, 999999);
        return "MEI" + medcatRand;
    }
    public static string GenerateNurseCode()
    {

        int nurRand = random.Next(100000, 999999);
        return "NUR" + nurRand;
    }

    public static string GeneratePatientCode()
    {

        int paRand = random.Next(100000, 999999);
        return "PAT" + paRand;
    }
    public static string GeneratePaymentCode()
    {

        int payRand = random.Next(100000, 999999);
        return "PAY" + payRand;
    }
    public static string GeneratePrescriptionCode()
    {

        int proRand = random.Next(100000, 999999);
        return "PRE" + proRand;
    }
    public static string GenerateRolesCode()
    {

        int roleRand = random.Next(100000, 999999);
        return "ROL" + roleRand;
    }
}

namespace SWP391_SE1914_ManageHospital.Service;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

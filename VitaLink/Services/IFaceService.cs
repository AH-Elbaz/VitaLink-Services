using VitaLink.Models; // تأكد من الـ Namespaces
using Vitalink.Models;

namespace Vitalink.API.Services
{
    public interface IFaceService
    {
        // التحقق من الوجه ومقارنته بالمستخدمين الحاليين
        Task<FaceRegistrationStatus> ValidateFaceForRegistration(Stream imageStream, List<AthleteProfile> existingUsers);

        // إنشاء البصمة الرقمية
        Task<byte[]?> GenerateFaceEncodingAsync(Stream imageStream);

        // التحقق من الهوية
        Task<bool> VerifyUserAsync(byte[] storedEncoding, Stream imageStream);
    }
}
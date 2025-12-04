// ملف: Controllers/AuthController.cs
using Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vitalink.API.Dtos;
using Vitalink.API.Services;
using Vitalink.Models;
using VitaLink.Models;
using VitaLink.Models.Data;

namespace Vitalink.API.Controllers
{


    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public class ResetPasswordDto
        {
            public string Username { get; set; }
            public string NewPassword { get; set; }
        
        }
        private readonly VitalinkDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IFaceService _faceService;

        public AuthController(VitalinkDbContext context, ITokenService tokenService , IFaceService faceService )
        {
            _context = context;
            _tokenService = tokenService;
            _faceService = faceService;
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            // 1. نجيب المستخدم
            var athlete = await _context.AthleteProfiles.FirstOrDefaultAsync(a => a.FirstName == dto.Username);
            if (athlete == null) return BadRequest("المستخدم غير موجود.");

            // 2. تحديث الباسورد
            // ملحوظة: هنا بنثق إن الفرونت إند مش هينادي الـ API دي غير لما يكون عدى على VerifyIdentity
            // لتأمين أقوى: المفروض VerifyIdentity ترجع Token وتستقبله هنا (بس للتبسيط هنمشيها كدة دلوقتي)

            athlete.PasswordHash = _tokenService.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            return Ok(new { Message = "تم تغيير كلمة المرور بنجاح. يمكنك تسجيل الدخول الآن." });
        }


        // المسار: POST /api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto credentials)
        {
            var athlete = await _context.AthleteProfiles
                .FirstOrDefaultAsync(a => a.FirstName == credentials.Username);

            // التحقق من وجود المستخدم وكلمة المرور
            if (athlete == null || !_tokenService.VerifyPassword(credentials.Password, athlete.PasswordHash))
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            // توليد التوكنات والحفظ
            var accessToken = _tokenService.GenerateAccessToken(athlete);
            var refreshToken = _tokenService.GenerateRefreshToken(athlete);
            await _tokenService.SaveRefreshTokenAsync(athlete, refreshToken);

            // إرجاع الاستجابة المطلوبة
            var responseDto = _tokenService.CreateTokenResponseDto(athlete, accessToken, refreshToken);
            return Ok(responseDto);
        }


        [HttpPost("register")]
        [ProducesResponseType(typeof(AthleteProfile), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)] // Conflict status code
        public async Task<ActionResult<AthleteProfile>> Register([FromBody] RegisterDto registerDto)
        {
            // أ. التحقق من أن الاسم غير مكرر (Logic القديم)
            var existingUser = await _context.AthleteProfiles
                .AnyAsync(a => a.FirstName == registerDto.FirstName);

            if (existingUser)
            {
                return BadRequest(new { Message = "Username (FirstName) is already taken." });
            }

            Guid? azurePersonId = null;

            // ب. التحقق من صورة الوجه (Logic الجديد)
            if (!string.IsNullOrEmpty(registerDto.ProfileImageBase64))
            {
                try
                {
                    using var imageStream = ConvertBase64ToStream(registerDto.ProfileImageBase64);

                    // 1. استدعاء دالة الفحص التي كتبناها في الـ Service
                    var validationStatus = await _faceService.ValidateFaceForRegistration(imageStream);

                    switch (validationStatus)
                    {
                        case FaceRegistrationStatus.NoFaceDetected:
                            return BadRequest(new { Message = "الصورة لا تحتوي على وجه واضح. يرجى التقاط صورة سيلفي واضحة." });

                        case FaceRegistrationStatus.MultipleFacesDetected:
                            return BadRequest(new { Message = "يوجد أكثر من شخص في الصورة. يجب أن تكون الصورة لك وحدك." });

                        case FaceRegistrationStatus.UserAlreadyExists:
                            // هنا منعنا التسجيل لأن الوجه موجود بالفعل
                            return Conflict(new { Message = "هذا الوجه مسجل بالفعل في النظام بحساب آخر!" });

                        case FaceRegistrationStatus.Error:
                            return StatusCode(500, new { Message = "حدث خطأ تقني أثناء فحص الصورة." });
                    }

                    // 2. إذا نجح الفحص، نعيد المؤشر لبداية الـ Stream لكي نقرأه مرة أخرى للتسجيل
                    imageStream.Position = 0;

                    // 3. إنشاء الشخص في Azure والحصول على الـ ID
                    azurePersonId = await _faceService.CreatePersonAndAddFaceAsync(registerDto.FirstName, imageStream);

                    if (azurePersonId == null)
                    {
                        return StatusCode(500, new { Message = "فشل في تسجيل بصمة الوجه في Azure." });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Message = "حدث خطأ في معالجة ملف الصورة. تأكد من إرسال Base64 صحيح." });
                }
            }
            else
            {
                // (اختياري) يمكنك جعل الصورة إجبارية بإلغاء التعليق عن السطر التالي
                // return BadRequest(new { Message = "صورة الوجه مطلوبة للتسجيل." });
            }

            // ج. تشفير كلمة المرور وحفظ المستخدم (Logic القديم مع إضافة AzurePersonId)
            string passwordHash = _tokenService.HashPassword(registerDto.Password);

            var newAthlete = new AthleteProfile
            {
                AthleteID = Guid.NewGuid().ToString(),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PasswordHash = passwordHash,
                Role = 0,
                BirthDate = registerDto.BirthDate,
                Weight = registerDto.Weight,
                BodyFatPercentage = registerDto.BodyFatPercentage,
                BloodType = registerDto.BloodType,
                TargetSport = registerDto.TargetSport,

                // هام: حفظنا الـ ID القادم من Azure
                AzurePersonId = azurePersonId
            };

            _context.AthleteProfiles.Add(newAthlete);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Login), new { username = newAthlete.FirstName }, newAthlete);
        }

        // 4. دالة مساعدة لتحويل النص (Base64) إلى ملف (Stream)
        private Stream ConvertBase64ToStream(string base64)
        {
            // إزالة الـ Header مثل "data:image/jpeg;base64," إذا كان موجوداً
            var base64String = base64;
            if (base64.Contains(","))
            {
                base64String = base64.Split(',')[1];
            }

            var bytes = Convert.FromBase64String(base64String);
            return new MemoryStream(bytes);
        }

        [HttpPost("verify-identity")]
        public async Task<IActionResult> VerifyIdentity([FromBody] VerifyIdentityDto dto)
        {
            // 1. حماية: التأكد من أن الصورة موجودة
            if (string.IsNullOrEmpty(dto.ImageBase64))
            {
                return BadRequest(new { Message = "صورة الوجه مطلوبة للتحقق." });
            }

            // 2. نجيب المستخدم من الداتابيز
            var athlete = await _context.AthleteProfiles
                .FirstOrDefaultAsync(a => a.FirstName == dto.Username);

            // التحقق أن المستخدم موجود ولديه بصمة وجه مسجلة أصلاً
            if (athlete == null || athlete.AzurePersonId == null)
            {
                return BadRequest(new { Message = "المستخدم غير موجود أو لم يقم بتسجيل بصمة وجه من قبل." });
            }

            try
            {
                // 3. تحويل الصورة وفحصها
                using var imageStream = ConvertBase64ToStream(dto.ImageBase64);

                // استدعاء الخدمة (التي تقارن الوجه الجديد بالـ ID المحفوظ)
                bool isMatch = await _faceService.VerifyUserAsync((Guid)athlete.AzurePersonId, imageStream);

                if (isMatch)
                {
                    // 4. سيناريو النجاح:
                    // نصيحة أمنية: هنا يفضل ترجع Token خاص (ResetToken) يستخدمه عشان يغير الباسورد
                    // حالياً هنرجع رسالة نجاح عادية
                    return Ok(new
                    {
                        Message = "Identity Verified",
                        IsVerified = true,
                        // ResetToken = "Generate_JWT_Token_Here" // دي خطوة متقدمة قدام
                    });
                }
                else
                {
                    return Unauthorized(new { Message = "فشل التحقق. الوجه غير مطابق لصاحب الحساب." });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { Message = "حدث خطأ في معالجة الصورة. تأكد من إرسال صورة سليمة." });
            }
        }

    }


}

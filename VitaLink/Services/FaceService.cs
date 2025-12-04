using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using VitaLink.Models;

public interface IFaceService
{
    Task InitializeGroupAsync(); // لإنشاء الجروب أول مرة
    Task<Guid?> CreatePersonAndAddFaceAsync(string name, Stream imageStream);
    Task<bool> VerifyUserAsync(Guid personId, Stream imageStream);
    Task<FaceRegistrationStatus> ValidateFaceForRegistration(Stream imageStream);
}

public class FaceService : IFaceService
{
    private readonly IFaceClient _faceClient;
    private readonly string _personGroupId;

    public FaceService(IConfiguration config)
    {
        var endpoint = config["FaceSettings:Endpoint"];
        var key = config["FaceSettings:Key"];
        _personGroupId = config["FaceSettings:PersonGroupId"]!;

        _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(key))
        {
            Endpoint = endpoint
        };
    }

    // 1. التأكد من أن الجروب موجود (يتم استدعاؤها مرة واحدة عند تشغيل التطبيق)
    public async Task InitializeGroupAsync()
    {
        try
        {
            await _faceClient.PersonGroup.GetAsync(_personGroupId);
        }
        catch (APIErrorException)
        {
            // إذا لم يكن موجوداً، نقوم بإنشائه
            await _faceClient.PersonGroup.CreateAsync(_personGroupId, "VitaLink Athletes");
        }
    }

    // في ملف FaceService.cs

    public async Task<FaceRegistrationStatus> ValidateFaceForRegistration(Stream imageStream)
    {
        try
        {
            // 1. الكشف عن الوجوه (Detect)
            var detectedFaces = await _faceClient.Face.DetectWithStreamAsync(
                imageStream,
                recognitionModel: RecognitionModel.Recognition04,
                detectionModel: DetectionModel.Detection03,
                returnFaceId: true); // مهم جداً عشان المقارنة

            // 2. فحص العدد
            if (detectedFaces.Count == 0) return FaceRegistrationStatus.NoFaceDetected;
            if (detectedFaces.Count > 1) return FaceRegistrationStatus.MultipleFacesDetected;

            // 3. فحص التكرار (هل الشخص ده موجود قبل كده؟)
            // ملاحظة: لازم الجروب يكون متدرب عشان الخطوة دي تشتغل
            var faceId = detectedFaces[0].FaceId;

            try
            {
                var identifyResults = await _faceClient.Face.IdentifyAsync(
                    new[] { faceId.Value },
                    _personGroupId);

                // لو لقينا شخص مطابق بنسبة ثقة عالية (أكتر من 70%)
                if (identifyResults.Any(r => r.Candidates.Any(c => c.Confidence > 0.7)))
                {
                    return FaceRegistrationStatus.UserAlreadyExists;
                }
            }
            catch (APIErrorException ex) when (ex.Body.Error.Code == "PersonGroupNotTrained")
            {
                // لو الجروب لسه جديد ومش متدرب، يبقى أكيد الشخص مش موجود، كمل عادي
            }

            return FaceRegistrationStatus.Success;
        }
        catch (Exception ex)
        {
            // سجل الخطأ هنا لو حابب
            return FaceRegistrationStatus.Error;
        }
    }

    // 2. التسجيل: إنشاء شخص وإضافة صورته
    public async Task<Guid?> CreatePersonAndAddFaceAsync(string name, Stream imageStream)
    {
        try
        {
            // أ. إنشاء "شخص" فارغ باسم المستخدم
            var person = await _faceClient.PersonGroupPerson.CreateAsync(_personGroupId, name);

            // ب. إضافة الصورة لهذا الشخص
            await _faceClient.PersonGroupPerson.AddFaceFromStreamAsync(
                _personGroupId,
                person.PersonId,
                imageStream
            );

            // ج. تدريب الموديل (مهم جداً عشان التحديث يسمع)
            await _faceClient.PersonGroup.TrainAsync(_personGroupId);

            return person.PersonId; // نرجع الـ ID عشان نخزنه في الداتابيز
        }
        catch
        {
            return null; // فشل العملية
        }
    }

    // 3. التحقق (نسيان الباسورد): مقارنة صورة جديدة بالـ ID المخزن
    public async Task<bool> VerifyUserAsync(Guid personId, Stream imageStream)
    {
        try
        {
            // أ. كشف الوجه في الصورة الجديدة (Detect)
            var detectedFaces = await _faceClient.Face.DetectWithStreamAsync(
                imageStream,
                recognitionModel: RecognitionModel.Recognition04,
                detectionModel: DetectionModel.Detection03);

            if (detectedFaces.Count == 0) return false; // مفيش وش في الصورة

            // ب. التحقق (Verify)
            var faceId = detectedFaces[0].FaceId; // ناخد أول وش
            var verifyResult = await _faceClient.Face.VerifyFaceToPersonAsync(
                (Guid)faceId!,
                personId,
                _personGroupId);

            return verifyResult.IsIdentical && verifyResult.Confidence > 0.7; // نسبة تطابق 70%
        }
        catch
        {
            return false;
        }
    }
}
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using VitaLink.Models;

public interface IFaceService
{
    Task InitializeGroupAsync();
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

   
    public async Task InitializeGroupAsync()
    {
        try
        {
            await _faceClient.PersonGroup.GetAsync(_personGroupId);
        }
        catch (APIErrorException)
        {
            await _faceClient.PersonGroup.CreateAsync(_personGroupId, "VitaLink Athletes");
        }
    }

  
    public async Task<FaceRegistrationStatus> ValidateFaceForRegistration(Stream imageStream)
    {
        try
        {
            var detectedFaces = await _faceClient.Face.DetectWithStreamAsync(
                imageStream,
                recognitionModel: RecognitionModel.Recognition04,
                detectionModel: DetectionModel.Detection03,
                returnFaceId: true);

            if (detectedFaces.Count == 0) return FaceRegistrationStatus.NoFaceDetected;
            if (detectedFaces.Count > 1) return FaceRegistrationStatus.MultipleFacesDetected;

            var faceId = detectedFaces[0].FaceId;

            try
            {
                var identifyResults = await _faceClient.Face.IdentifyAsync(
                    new[] { faceId.Value },
                    _personGroupId);

                if (identifyResults.Any(r => r.Candidates.Any(c => c.Confidence > 0.7)))
                {
                    return FaceRegistrationStatus.UserAlreadyExists;
                }
            }
            catch (APIErrorException ex) when (ex.Body.Error.Code == "PersonGroupNotTrained")
            {
            }

            return FaceRegistrationStatus.Success;
        }
        catch (Exception ex)
        {
            return FaceRegistrationStatus.Error;
        }
    }


    public async Task<Guid?> CreatePersonAndAddFaceAsync(string name, Stream imageStream)
    {
        try
        {
            var person = await _faceClient.PersonGroupPerson.CreateAsync(_personGroupId, name);

            await _faceClient.PersonGroupPerson.AddFaceFromStreamAsync(
                _personGroupId,
                person.PersonId,
                imageStream
            );

            await _faceClient.PersonGroup.TrainAsync(_personGroupId);

            return person.PersonId;
        }
        catch
        {
            return null;
        }
    }


    public async Task<bool> VerifyUserAsync(Guid personId, Stream imageStream)
    {
        try
        {
            var detectedFaces = await _faceClient.Face.DetectWithStreamAsync(
                imageStream,
                recognitionModel: RecognitionModel.Recognition04,
                detectionModel: DetectionModel.Detection03);

            if (detectedFaces.Count == 0) return false;

            var faceId = detectedFaces[0].FaceId;
            var verifyResult = await _faceClient.Face.VerifyFaceToPersonAsync(
                (Guid)faceId!,
                personId,
                _personGroupId);

            return verifyResult.IsIdentical && verifyResult.Confidence > 0.7;
        }
        catch
        {
            return false;
        }
    }
}
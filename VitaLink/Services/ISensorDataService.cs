using Vitalink.API.Dtos;

namespace Vitalink.API.Services
{
    public interface ISensorDataService
    {
       
        Task SaveRowData(SensorDataDto data);
    }
}
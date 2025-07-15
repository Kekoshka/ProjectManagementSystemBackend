using Mapster;
using ProjectManagementSystemBackend.Models;

namespace ProjectManagementSystemBackend.Common
{
    public class MapsterConfig : TypeAdapterConfig
    {
        public MapsterConfig() 
        {
            ForType<BoardStatus, Status>()
                .Map(dest => dest.Name, src => src.Status.Name);
        }
    }
}

using Microsoft.Graph;

namespace CloudReservation.Service.Services.GraphServices.Builder;

public interface IGraphBuilder
{
    public GraphServiceClient Build();
}
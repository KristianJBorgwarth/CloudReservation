using Newtonsoft.Json;
// ReSharper disable All

namespace CloudReservation.Service.Models.NotificationModels;

public class NotificationWrapper
{
    public required List<Notification> Value { get; init; } = new();
}

public class Notification
{
    public string SubscriptionId { get; init; }
    public DateTime SubscriptionExpirationDateTime { get; init; }
    public string ChangeType { get; init; }
    public string Resource { get; init; }
    public ResourceData ResourceData { get; init; }
    public string ClientState { get; init; }
    public string TenantId { get; init; }
}

public class ResourceData
{
    // The ID of the resource.
    [JsonProperty(PropertyName = "id")] public string Id { get; init; }

    // The OData etag property.
    [JsonProperty(PropertyName = "@odata.etag")]
    public string ODataEtag { get; init; }

    // The OData ID of the resource. This is the same value as the resource property.
    [JsonProperty(PropertyName = "@odata.id")]
    public string ODataId { get; init; }

    // The OData type of the resource: "#Microsoft.Graph.Message", "#Microsoft.Graph.Event", or "#Microsoft.Graph.Contact".
    [JsonProperty(PropertyName = "@odata.type")]
    public string ODataType { get; init; }
}
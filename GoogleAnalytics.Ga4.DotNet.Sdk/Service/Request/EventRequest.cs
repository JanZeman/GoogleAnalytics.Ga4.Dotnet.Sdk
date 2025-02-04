using System.Text.Json;
using System.Text.Json.Serialization;
using GoogleAnalytics.Ga4.DotNet.Sdk.Service.Client;
using GoogleAnalytics.Ga4.DotNet.Sdk.Service.Response;

namespace GoogleAnalytics.Ga4.DotNet.Sdk.Service.Request;


public class EventRequest : IEventRequest
{
    public IBasicHttpClient Client { get; set; }
    
    [JsonPropertyName("client_id")] 
    public string ClientId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_id")] 
    public string UserId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("timestamp_micros")] 
    public int TimestampMicros { get; set; }

    [JsonPropertyName("non_personalized_ads")]
    public bool NonPersonalizedAds { get; set; }

    [JsonPropertyName("events")] 
    public List<IEvent> Events { get; set; }
    
    
    public EventRequest(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException(nameof(clientId));
        
        var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
        var secondsSinceEpoch = (int)t.TotalSeconds;
        
        TimestampMicros = secondsSinceEpoch;
        ClientId = clientId;
    }
    public EventRequest(string clientId, string userId) :this(clientId)
    {
        UserId = userId;
    }
    public EventRequest(string clientId, string userId, bool nonPersonalizedAds = false) :this(clientId, userId)
    {
        NonPersonalizedAds = nonPersonalizedAds;
    }
    public async Task<EventResponse> Send(bool debug =false)
    {
        var path = "/mp/collect";
        
        if(debug) path = "/debug/mp/collect";
        
        var response = await Client.PostAsync(path, this);
        var responseData = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<DebugResponse>(responseData);

        return new EventResponse()
        {
            IsSuccess = response.IsSuccessStatusCode,
            Message = response,
            DebugResponse = (debug)? JsonSerializer.Deserialize<DebugResponse>(responseData) : null
        };
    }

    public void SetClient(IBasicHttpClient client)
    {
        Client = client;
    }
}
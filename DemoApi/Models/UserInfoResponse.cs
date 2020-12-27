using AspNet.Security.OpenIdConnect.Primitives;
using System.Text.Json.Serialization;

namespace DemoApi.Models
{
    public class UserInfoResponse : Resource
    {
        [JsonPropertyName(OpenIdConnectConstants.Claims.Subject)]
        public string Subject { get; set; }

        [JsonPropertyName(OpenIdConnectConstants.Claims.GivenName)]
        public string GivenName { get; set; }

        [JsonPropertyName(OpenIdConnectConstants.Claims.FamilyName)]
        public string FamilyName { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace Exadel.BookManagement.API.Helpers
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumHelper
    {
        Default,
        Title
    }
}

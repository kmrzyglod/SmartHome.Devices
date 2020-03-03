using Json.NetMF;

namespace EspIot.Core.Messaging.Validation
{
    public static class ValidationErrorExtensions
    {
        public static string SerializeToString(this ValidationError[] errors)
        {
            return JsonSerializer.SerializeObject(errors);
        }
    }
}
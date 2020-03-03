using Json.NetMF;

namespace EspIot.Core.Messaging.Validation
{
    public class ValidationError
    {
        public ValidationError(string fieldName, string message)
        {
            FieldName = fieldName;
            Message = message;
        }

        public string FieldName { get; }
        public string Message { get; }

        public override string ToString()
        {
            return JsonSerializer.SerializeObject(this);
        }
    }
}

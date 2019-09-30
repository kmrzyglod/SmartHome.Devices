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
    }
}

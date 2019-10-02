using System.Collections;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Validation;

namespace Infrastructure.Commands.Command
{
    internal class OpenWindowCommand : CommandBase
    {
        public ushort[] WindowIds { get; }

        public OpenWindowCommand(string correlationId, ushort[] windowIds) : base(correlationId)
        {
            WindowIds = windowIds;
        }

        public override ValidationError[] Validate()
        {
            var errors = ValidateBase();

            if (WindowIds == null || WindowIds.Length == 0)
            {
                errors.Add(new ValidationError(nameof(WindowIds), $"{nameof(WindowIds)} field cannot be empty"));
            }

            return ConvertToArray(errors);
        }

        public static OpenWindowCommand FromHashtable(Hashtable obj)
        {
            return new OpenWindowCommand(obj.GetString(nameof(CorrelationId)), obj.GetUShortList(nameof(WindowIds)));
        }
    }
}
using System.Collections;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Validation;
using GreenhouseController.Application.Services.WindowsManager;

namespace GreenhouseController.Application.Commands.CloseWindow
{
    public class CloseWindowCommand : CommandBase
    {
        public ushort[] WindowIds { get; }

        public CloseWindowCommand(string correlationId, ushort[] windowIds) : base(correlationId)
        {
            WindowIds = windowIds;
        }

        public override string PartitionKey { get; } = nameof(WindowsManagerService);

        public override ValidationError[] Validate()
        {
            var errors = ValidateBase();

            if (WindowIds == null || WindowIds.Length == 0)
            {
                errors.Add(new ValidationError(nameof(WindowIds), $"{nameof(WindowIds)} field cannot be empty"));
            }

            return ConvertToArray(errors);
        }

        public static CloseWindowCommand FromHashtable(Hashtable obj)
        {
            return new CloseWindowCommand(obj.GetString(nameof(CorrelationId)), obj.GetUShortList(nameof(WindowIds)));
        }

        public class Factory
        {
            public static CloseWindowCommand Create(Hashtable obj)
            {
                return FromHashtable(obj);
            }
        }
    }
}
using System.Collections;
using WindowsController.Application.Services;
using EspIot.Application.Commands;
using EspIot.Application.Interfaces;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Validation;

namespace WindowsController.Application.Commands.CloseWindow
{
    public class CloseWindowCommand : CommandBase
    {
        public const ushort MAX_WINDOW_ID = 2;

        public CloseWindowCommand(string correlationId, ushort windowId) : base(correlationId)
        {
            WindowId = windowId;
        }

        public CloseWindowCommand(string correlationId) : base(correlationId)
        {
        }

        public ushort WindowId { get; }

        public override string PartitionKey => $"{nameof(WindowsManagingService)}__{WindowId}";

        public override ValidationError[] Validate()
        {
            var errors = ValidateBase();
            if (WindowId > MAX_WINDOW_ID)
            {
                errors.Add(new ValidationError(nameof(WindowId),
                    $"{nameof(WindowId)} field cannot has value higher than {MAX_WINDOW_ID}"));
            }

            return ConvertToArray(errors);
        }

        public static CloseWindowCommand FromHashtable(Hashtable obj)
        {
            var command = new CloseWindowCommand(obj.GetString(nameof(CorrelationId)), obj.GetUShort(nameof(WindowId)));
            return command;
        }

        public class Factory : ICommandFactory
        {
            ICommand ICommandFactory.Create(Hashtable obj)
            {
                return FromHashtable(obj);
            }
        }
    }
}
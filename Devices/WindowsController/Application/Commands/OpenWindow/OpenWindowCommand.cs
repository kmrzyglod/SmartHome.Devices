using System.Collections;
using WindowsController.Application.Services;
using EspIot.Application.Commands;
using EspIot.Application.Interfaces;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Validation;

namespace WindowsController.Application.Commands.OpenWindow
{
    public class OpenWindowCommand : CommandBase
    {
        public const ushort MAX_WINDOW_ID = 1;

        public OpenWindowCommand(string correlationId, ushort windowId) : base(correlationId)
        {
            WindowId = windowId;
        }

        public override string PartitionKey => $"{nameof(WindowsManagingService)}__{WindowId}";
        public ushort WindowId { get; }

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

        public static OpenWindowCommand FromHashtable(Hashtable obj)
        {
            var command = new OpenWindowCommand(obj.GetString(nameof(CorrelationId)), obj.GetUShort(nameof(WindowId)));
            return command;
        }

        public class Factory : ICommandFactory
        {
            public ICommand Create(Hashtable obj)
            {
                return FromHashtable(obj);
            }
        }
    }
}
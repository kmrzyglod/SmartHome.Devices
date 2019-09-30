using System;
using System.Collections;
using EspIot.Core.Messaging.Interfaces;
using EspIot.Core.Messaging.Validation;

namespace Infrastructure.Commands.Command
{
    class CloseWindowCommand : ICommand
    {
        public ushort[] WindowIds { get;}
        public string CorrelationId { get; }
        
        public CloseWindowCommand(string correlationId, ushort[] windowIds)
        {
            CorrelationId = correlationId;
            WindowIds = windowIds;
        }

        public ValidationError[] Validate()
        {
            var errors = new ArrayList();
            
            if (CorrelationId == null || CorrelationId == string.Empty)
            {
                errors.Add(new ValidationError(nameof(CorrelationId), $"{nameof(CorrelationId)} field cannot be empty"));
            }

            if (WindowIds == null || WindowIds.Length == 0)
            {
                errors.Add(new ValidationError(nameof(WindowIds), $"{nameof(WindowIds)} field cannot be empty"));
            }

            return errors.ToArray(typeof(ValidationError)) as ValidationError[];
        }

        public static CloseWindowCommand FromHashtable(Hashtable obj)
        {
            return new CloseWindowCommand(obj[nameof(CorrelationId)] as string, obj[nameof(WindowIds)] as ushort[]);
        }

    }
}

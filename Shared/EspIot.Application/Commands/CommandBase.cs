using System.Collections;
using EspIot.Application.Interfaces;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Validation;

namespace EspIot.Application.Commands
{
    public abstract class CommandBase: ICommand
    {
        public string CorrelationId { get; }
        public abstract string PartitionKey { get; }

        protected CommandBase(string correlationId)
        {
            CorrelationId = correlationId;
        }

        public virtual ValidationError[] Validate()
        {
            var errors = ValidateBase();

            return ConvertToArray(errors);
        }
        
        protected ArrayList ValidateBase()
        {
            var errors = new ArrayList();

            if (CorrelationId.IsNullOrEmpty())
            {
                errors.Add(new ValidationError(nameof(CorrelationId),
                    $"{nameof(CorrelationId)} field cannot be empty"));
            }

            return errors;
        }

        protected ValidationError[] ConvertToArray(ArrayList validationErrors)
        {
            return validationErrors.ToArray(typeof(ValidationError)) as ValidationError[];
        }
    }
}

using System;
using System.Collections;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Interfaces;
using EspIot.Core.Messaging.Validation;

namespace EspIot.Core.Messaging.Concrete
{
    public abstract class CommandBase: ICommand
    {
        public string CorrelationId { get; }

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

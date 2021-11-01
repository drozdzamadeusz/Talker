using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace talker.Application.Common.Exceptions
{
    public class InvalidOperation : Exception
    {
        public InvalidOperation()
            : base("Invalid operation")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public InvalidOperation(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}
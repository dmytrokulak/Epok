using System;

namespace Epok.Core.Domain.Exceptions
{
    /// <summary>
    /// Exception raised in response of violation
    /// of domain rules.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }
}

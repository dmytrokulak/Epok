using Epok.Core.Domain.Entities;
using Epok.Domain.Contacts.Entities;
using System;

namespace Epok.Domain.Payroll.Entities
{
    /// <summary>
    /// ToDo:5 develop payroll management.
    /// For now keep this class to avoid temptation
    /// of putting irrelevant properties on User entity.
    /// </summary>
    public class Employee : EntityBase
    {
        public Employee(Guid id, string name) : base(id, name)
        {
        }

        /// <summary>
        /// Contact info: person name, email and phone.
        /// </summary>
        public virtual Contact Contact { get; set; }

        /// <summary>
        /// Position within the company operating the system.
        /// </summary>
        public virtual string Position { get; set; }

        public virtual decimal Salary { get; set; }
        public virtual Address Address { get; internal set; }

    }
}

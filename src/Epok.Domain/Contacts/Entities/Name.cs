using System;

namespace Epok.Domain.Contacts.Entities
{
    /// <summary>
    /// Person name as a single entity.
    /// </summary>
    [Serializable]
    public struct PersonName
    {
        public PersonName(string firstName, string lastName,
            string middleName = "")
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string MiddleName { get; }
        public string LastName { get; }

        public bool Equals(PersonName other)
        {
            return FirstName == other.FirstName
                   && MiddleName == other.MiddleName
                   && LastName == other.LastName;
        }

        public override bool Equals(object obj)
        {
            return obj is PersonName other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MiddleName != null ? MiddleName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}

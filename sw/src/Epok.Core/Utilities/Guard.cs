using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Epok.Core.Utilities
{
    public static class Guard
    {
        public static class Against
        {
            public static void Null(object input, string name)
            {
                if (input is null)
                    throw new ArgumentNullException(name);
            }

            public static void Zero(decimal input, string name)
            {
                if (input == 0)
                    throw new ArgumentException(name);
            }

            public static void Negative(decimal input, string name)
            {
                if (input < 0)
                    throw new ArgumentException(name);
            }

            public static void Empty(Guid input, string name)
            {
                if (input == Guid.Empty)
                    throw new ArgumentException(name);
            }

            public static void NullOrWhitespace(string input, string name)
            {
                if (string.IsNullOrWhiteSpace(input))
                    throw new ArgumentException(name);
            }

            // https://regexr.com/2rhq7
            private static readonly Regex EmailRegex
                = new Regex(
                    @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");

            public static void InvalidEmail(string input, string name)
            {
                if (!EmailRegex.IsMatch(input))
                    throw new ArgumentException(name);
            }

            public static void InvalidPhoneNumber(string input, string name)
            {
                if (!char.IsNumber(input.First()) && input.First() != '+')
                    throw new ArgumentException(name);
                if (input.Substring(1).Any(c => !char.IsNumber(c) && c != '-' && c != '(' && c != ')'))
                    throw new ArgumentException(name);
                if (input.Count(c => c == '(') != input.Count(c => c == ')'))
                    throw new ArgumentException(name);
                if (input.IndexOf(')') < input.IndexOf('(') || input.LastIndexOf('(') > input.LastIndexOf(')'))
                    throw new ArgumentException(name);
                if (!char.IsNumber(input.Last()))
                    throw new ArgumentException(name);
                if (input.Count(char.IsNumber) > 15)
                    throw new ArgumentException(name);
            }
        }
    }
}

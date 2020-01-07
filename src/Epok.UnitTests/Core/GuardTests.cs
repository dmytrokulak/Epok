using Epok.Core.Utilities;
using NUnit.Framework;
using System;

namespace Epok.UnitTests.Core
{
    [TestFixture]
    public class GuardTests
    {
        [TestCase(null, false)]
        [TestCase("notnull", true)]
        public void CanGuardAgainstNull(object input, bool valid)
        {
            if (!valid)
                Assert.Throws<ArgumentNullException>(() => Guard.Against.Null(input, nameof(input)));
            else
                Assert.DoesNotThrow(() => Guard.Against.Null(input, nameof(input)));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void CanGuardAgainstZero(int input, bool valid)
        {
            if (!valid)
                Assert.Throws<ArgumentException>(() => Guard.Against.Zero(input, nameof(input)));
            else
                Assert.DoesNotThrow(() => Guard.Against.Zero(input, nameof(input)));
        }

        [TestCase(-1, false)]
        [TestCase(1, true)]
        [TestCase(0, true)]
        public void CanGuardAgainstNegative(int input, bool valid)
        {
            if (!valid)
                Assert.Throws<ArgumentException>(() => Guard.Against.Negative(input, nameof(input)));
            else
                Assert.DoesNotThrow(() => Guard.Against.Negative(input, nameof(input)));
        }

        [TestCase("00000000-0000-0000-0000-000000000000", false)]
        [TestCase("12300000-0000-0000-0000-000000000012", true)]
        public void CanGuardAgainstEmtpyGuid(Guid input, bool valid)
        {
            if (!valid)
                Assert.Throws<ArgumentException>(() => Guard.Against.Empty(input, nameof(input)));
            else
                Assert.DoesNotThrow(() => Guard.Against.Empty(input, nameof(input)));
        }

        /// <summary>
        /// https://blogs.msdn.microsoft.com/testing123/2009/02/06/email-address-test-cases/
        /// </summary>
        [TestCase("mail@me.com", true)]
        [TestCase("send.mail@me.com", true)]
        [TestCase("1send1.mail1@me.com", true)]
        [TestCase("send1.mail@me.com.ua", true)]
        [TestCase("send+mail@me.com.ua", true)]
        [TestCase("mail@me-you.com", true)]
        [TestCase("send-mail@me.com", true)]
        [TestCase("send_mail@me.com", true)]
        [TestCase("12345@me.com", true)]
        [TestCase("s-e=n_d+m!a#i$l%@me.com", true)]
        [TestCase("#@%#$#$@me.com", false)]
        [TestCase(".mail1@me.com", false)]
        [TestCase("mail1@me", false)]
        [TestCase("mail@at@me.com", false)]
        [TestCase("mail..at@me.com", false)]
        [TestCase("mail.at@me..com", false)]
        [TestCase("mail.at@-me.com", false)]
        [TestCase("邮局@me.com", false)]
        [TestCase("почта@me.com", false)]
        [TestCase("@me.com", false)]

        public void CanGuardAgainstInvalidEmail(string input, bool valid)
        {
            if (!valid)
                Assert.Throws<ArgumentException>(() => Guard.Against.InvalidEmail(input, nameof(input)));
            else
                Assert.DoesNotThrow(() => Guard.Against.InvalidEmail(input, nameof(input)));
        }

        [TestCase("+7(926)1234567", true)]
        [TestCase("+7(926)123-45-67", true)]
        [TestCase("+7(926)12-34-567", true)]
        [TestCase("7(926)1234567", true)]
        [TestCase("79261234567", true)]
        [TestCase("+79261234567", true)]
        [TestCase("+7-926-12-34-567", true)]
        [TestCase("+7-926-123-45-67", true)]
        [TestCase("+7(926)1234567-a", false)]
        [TestCase("7 926 1234567", false)]
        [TestCase("telephone123", false)]
        public void CanGuardAgainstInvalidPhoneNumber(string input, bool valid)
        {
            if (!valid)
                Assert.Throws<ArgumentException>(() => Guard.Against.InvalidPhoneNumber(input, nameof(input)));
            else
                Assert.DoesNotThrow(() => Guard.Against.InvalidPhoneNumber(input, nameof(input)));
        }
    }
}


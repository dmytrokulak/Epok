using System;
using System.Linq;

namespace Epok.Integration.Tests
{
    internal static class TestHelper
    {
        internal static T RandomEnumValue<T>(T excluding, bool includeDefault = false) where T : Enum
        {
            return Enum.GetValues(typeof(T)).OfType<T>()
                .First(v =>  (includeDefault || Convert.ToInt32(v) != 0) && !v.Equals(excluding));
        }
    }
}

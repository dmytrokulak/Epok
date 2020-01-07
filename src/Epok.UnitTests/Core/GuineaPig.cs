using Epok.Core.Domain.Entities;
using System;

namespace Epok.UnitTests.Core
{
    [Serializable]
    internal class GuineaPig : EntityBase
    {
        internal GuineaPig(string name, double weight, double bodyTemp)
            : base(Guid.NewGuid(), name)
        {
            Name = name;
            Weight = weight;
            BodyTemp = bodyTemp;
        }

        internal double Weight { get; }
        private double BodyTemp { get; }
    }
}

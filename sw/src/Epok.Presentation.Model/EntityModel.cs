using System;
using AutoMapper;
using Epok.Core.Domain.Entities;

namespace Epok.Presentation.Model
{
    /// <summary>
    /// Shortcut for displaying entities.
    /// </summary>
    public class EntityModel
    {
        //EntityBase
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<EntityBase, EntityModel>();
            }
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

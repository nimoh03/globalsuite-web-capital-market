#pragma warning disable CS8618
using System;

namespace GlobalSuite.Core.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : AppException
    {
        public Type EntityType { get; set; }
        public object Id { get; set; }

        public EntityNotFoundException()
          : base("Entity not found.")
        {
        }

        public EntityNotFoundException(string message)
          : base(message)
        {
        }

        public EntityNotFoundException(Type entityType, object id, Exception innerException)
          : base($"There is no such an entity. Entity type: {entityType.FullName}, id: {id}", innerException)
        {
            EntityType = entityType;
            Id = id;
        }

        public EntityNotFoundException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        public EntityNotFoundException(Type entityType, object id) : base(
          $"There is no such an entity. Entity type: {entityType.FullName}, id: {id}")
        {
        }
    }
}
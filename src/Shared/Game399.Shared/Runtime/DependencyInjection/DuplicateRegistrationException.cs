using System;

namespace Game399.Shared.DependencyInjection
{
    /// <summary>
    /// Thrown when a type is registered more than once in the container.
    /// </summary>
    public sealed class DuplicateRegistrationException : InvalidOperationException
    {
        public Type RegisteredType { get; }

        public DuplicateRegistrationException(Type type)
            : base($"Type {type.FullName} is already registered.")
        {
            RegisteredType = type;
        }
    }
}
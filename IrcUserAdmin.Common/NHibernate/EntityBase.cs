using System;

namespace IrcUserAdmin.Common.NHibernate
{
    public abstract class EntityBase<T> : IEntity where T : EntityBase<T> 
    {
        private int? _oldHashCode;

        public virtual Guid Id { get; set; }

        public override int GetHashCode()
        {
            // Once we have a hash code we'll never change it
            if (_oldHashCode.HasValue)
                return _oldHashCode.Value;
            bool thisIsTransient = Equals(Id, Guid.Empty);
            // When this instance is transient, we use the base GetHashCode()
            // and remember it, so an instance can NEVER change its hash code.
            if (thisIsTransient)
            {
                _oldHashCode = base.GetHashCode();
                return _oldHashCode.Value;
            }
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as T;
            if (other == null)
                return false;
            // handle the case of comparing two NEW objects
            bool otherIsTransient = Equals(other.Id, Guid.Empty);
            bool thisIsTransient = Equals(Id, Guid.Empty);
            if (otherIsTransient && thisIsTransient)
                return ReferenceEquals(other, this);
            return other.Id.Equals(Id);
        }

        public static bool operator ==(EntityBase<T> x, EntityBase<T> y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(EntityBase<T> x, EntityBase<T> y)
        {
            return !Equals(x, y);
        }
    }
}
using System.Diagnostics.CodeAnalysis;

namespace TenantX.Common.Domain.Shared;

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
public abstract record EntityId<T>(Guid Value) where T : EntityId<T>
{
  public static T New() => (T)Activator.CreateInstance(typeof(T), Guid.NewGuid())!;
  public static T From(Guid value) => (T)Activator.CreateInstance(typeof(T), value)!;
  public override string ToString() => Value.ToString();

  public static implicit operator Guid(EntityId<T> entityId) => entityId.Value;
}
// simple way but we have to implement these methods in each derived record

// public abstract record EntityId<T>(Guid Value)
// {
//     public override string ToString() => Value.ToString();
//     public static implicit operator Guid(EntityId<T> id) => id.Value;
// }

// safest way without reflection and activator, but we have to implement these methods in each derived record

// public interface IStronglyTypedId<TSelf> where TSelf : IStronglyTypedId<TSelf>
// {
//     static abstract TSelf New();
//     static abstract TSelf From(Guid value);
//     Guid Value { get; }
// }

// public abstract record EntityId(Guid Value)
// {
//   public override string ToString() => Value.ToString();

//   public static implicit operator Guid(EntityId id) => id.Value;
// }
// public record WaleedId(Guid Value) : EntityId(Value), IStronglyTypedId<WaleedId>
// {

// 		// COMPILER ERROR if you delete either of these methods!
// 		public static WaleedId New() => new(Guid.NewGuid());
    
//     public static WaleedId From(Guid value) => new(value);
// }
namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class ValueObject<TValueObject, TValue> where TValueObject : ValueObject<TValueObject, TValue>
{
	protected ValueObject(TValue value)
	{
		Value = value;

		Validate();
	}

	public TValue Value { get; }

	protected abstract void Validate();
}

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class ValueObject<TValueObject, TValue> : ValueObject<TValue>
	where TValueObject : ValueObject<TValueObject, TValue>
{
	protected ValueObject()
	{
	}

	protected ValueObject(TValue value)
		: base(value)
	{
	}

	public static implicit operator TValue(ValueObject<TValueObject, TValue> value) => value.Value;
}

public abstract class ValueObject<TValue>
{
	public TValue Value { get; private set; }

	protected ValueObject()
	{
		Value = default!;
	}

	protected ValueObject(TValue value)
	{
		Value = value;

		ExecuteValidation();
	}

	protected abstract void Validate();

	private void ExecuteValidation()
	{
		Validate();
	}
}

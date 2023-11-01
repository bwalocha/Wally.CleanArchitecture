namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class ValueObject<TValueObject, TValue> where TValueObject : ValueObject<TValueObject, TValue>
{
	protected ValueObject(TValue value)
	{
		Value = value;

		ExecuteValidation();
	}

	public TValue Value { get; }

	private void ExecuteValidation()
	{
		Validate();
	}

	protected abstract void Validate();
}

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
	
	public static implicit operator TValue(ValueObject<TValueObject, TValue> value)
	{
		return value.Value;
	}
}

public abstract class ValueObject<TValue>
{
	protected ValueObject()
	{
		Value = default!;
	}
	
	protected ValueObject(TValue value)
	{
		Value = value;
		
		ExecuteValidation();
	}
	
	public TValue Value { get; private set; }
	
	protected abstract void Validate();
	
	private void ExecuteValidation()
	{
		Validate();
	}
}

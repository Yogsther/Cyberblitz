using System;

public struct UnitID
{
	private readonly string value;
	public UnitID(string value)
	{
		this.value = value;
	}

	public static UnitID New => new UnitID(Guid.NewGuid().ToString());

	public override string ToString()
	{
		return value;
	}
	public string Value { get { return value; } }

	public static implicit operator UnitID(string s)
	{
		return new UnitID(s);
	}

	public static implicit operator string(UnitID p)
	{
		return p.Value;
	}
}
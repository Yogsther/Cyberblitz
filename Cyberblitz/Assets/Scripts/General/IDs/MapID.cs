using System;

public struct MapID
{
	private readonly string value;
	public MapID(string value)
	{
		this.value = value;
	}

	public override string ToString()
	{
		return value;
	}
	public string Value { get { return value; } }

	public static implicit operator MapID(string s)
	{
		return new MapID(s);
	}

	public static implicit operator string(MapID p)
	{
		return p.Value;
	}
}
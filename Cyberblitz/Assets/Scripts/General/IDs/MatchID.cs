using System;

public struct MatchID
{
	private readonly string value;
	public MatchID(string value)
	{
		this.value = value;
	}
	public static MatchID New => new MatchID(Guid.NewGuid().ToString());

	public override string ToString()
	{
		return value;
	}
	public string Value { get { return value; } }

	public static implicit operator MatchID(string s)
	{
		return new MatchID(s);
	}

	public static implicit operator string(MatchID p)
	{
		return p.Value;
	}
}
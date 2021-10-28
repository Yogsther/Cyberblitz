using System;

public struct TimerID
{
	private readonly string value;
	public TimerID(string value)
	{
		this.value = value;
	}

	public static TimerID New => new TimerID(Guid.NewGuid().ToString());

	public override string ToString()
	{
		return value;
	}
	public string Value { get { return value; } }

	public static implicit operator TimerID(string s)
	{
		return new TimerID(s);
	}

	public static implicit operator string(TimerID p)
	{
		return p.Value;
	}
}
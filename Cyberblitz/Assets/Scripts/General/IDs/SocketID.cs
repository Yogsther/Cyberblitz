using System;

public struct SocketID
{
	private readonly string value;
	public SocketID(string value)
	{
		this.value = value;
	}

	public static SocketID New => new SocketID(Guid.NewGuid().ToString());

	public override string ToString()
	{
		return value;
	}
	public string Value { get { return value; } }

	public static implicit operator SocketID(string s)
	{
		return new SocketID(s);
	}

	public static implicit operator string(SocketID p)
	{
		return p.Value;
	}
}
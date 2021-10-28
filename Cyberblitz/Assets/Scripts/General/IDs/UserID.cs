using System;

public struct UserID
{
	private readonly string value;
	public UserID(string value)
	{
		this.value = value;
	}

	public static UserID New => new UserID(Guid.NewGuid().ToString());

	public override string ToString()
	{
		return value;
	}
	public string Value { get { return value; } }

	public static implicit operator UserID(string s)
	{
		return new UserID(s);
	}

	public static implicit operator string(UserID p)
	{
		return p.Value;
	}
}
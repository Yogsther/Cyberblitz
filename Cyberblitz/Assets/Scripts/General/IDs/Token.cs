using System;

public struct Token
{
	private readonly string value;
	public Token(string value)
	{
		this.value = value;
	}

	public static Token New => new Token(Guid.NewGuid().ToString());

	public override string ToString()
	{
		return value;
	}
	public string Value { get { return value; } }

	public static implicit operator Token(string s)
	{
		return new Token(s);
	}

	public static implicit operator string(Token p)
	{
		return p.Value;
	}
}
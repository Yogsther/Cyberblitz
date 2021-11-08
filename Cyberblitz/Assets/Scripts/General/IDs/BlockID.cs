using System;

public struct BlockID
{
	private readonly string value;
	public BlockID(string value)
	{
		this.value = value;
	}

	public static BlockID New => new BlockID(Guid.NewGuid().ToString());

	public override string ToString()
	{
		return value;
	}
	public string Value { get { return value; } }

	public static implicit operator BlockID(string s)
	{
		return new BlockID(s);
	}

	public static implicit operator string(BlockID p)
	{
		return p.Value;
	}
}
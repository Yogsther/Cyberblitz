[System.Serializable]
public abstract class Block
{
	public BlockID id = BlockID.New;
	public BlockType type;
	public float duration = 5f;
	public UnitID ownerId = "[ID NOT SET]";
	public int timelineIndex = -1;
	/*public bool firstPlaybackTick = true;*/

	public Block(UnitID ownerId, int timelineIndex)
	{
		BlockData template = BlockDataLoader.GetBlockData(type);
		duration = template.minLength;
		this.timelineIndex = timelineIndex;
		this.ownerId = ownerId;

	}

	public abstract void Simulate(Match simulatedMatch, float localTime);

	public abstract void OnPlaybackStart(Match simulatedMatch);

	public abstract void OnPlaybackEnd(Match simulatedMatch);

	public abstract void Playback(Match simulatedMatch, float localTime);

}
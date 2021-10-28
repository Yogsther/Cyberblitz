using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

	[ContextMenu("Tests/Test deserialize block")]
	public void BlockDeserialize()
	{
		Debug.Log("Creating block");
		GuardBlock block = new GuardBlock("test", 0);

		NetworkPacket packet = new NetworkPacket("test", block);
		string json = packet.ToJSON();
		Debug.Log("Created json from block");
		Debug.Log(json);

		packet.Parse<Block>();
		Debug.Log("Parsed block successfully");

		Debug.Log("Parsed block type: " + block.GetType());
		Debug.Log("Parsed block BlockType: " + block.type);
	}

	[ContextMenu("Tests/Serialize timeline")]
	public void TimelineTest()
	{

		User user = new User();
		user.username = "TEST_USER";

		Match match = new Match();
		MatchManager.match = match;
		match.state = Match.GameState.Planning;

		match.players = new Player[1];

		Player player = new Player(user, 0);
		match.players[0] = player;

		Unit unit = new Unit(user.id);

		player.units = new Unit[1];

		player.units[0] = unit;

		Debug.Log("Created unit " + unit.id);

		/*unit.timeline.ownerId = user*/

		GuardBlock guardBlock = new GuardBlock(unit.id, 0);
		Debug.Log("Created guard block");

		unit.timeline.InsertBlock(guardBlock);
		Debug.Log("Inserted guard block");

		MoveBlock moveBlock = new MoveBlock(unit.id, 1);
		Debug.Log("Created move block");

		GridPoint origin = new GridPoint(new Vector2Int(0, 0));
		GridPoint target = new GridPoint(new Vector2Int(5, 5));
		moveBlock.movementPath.origin = origin;
		moveBlock.movementPath.target = target;

		moveBlock.movementPath = new AutoGridPath(origin);
		foreach (Vector2Int point in moveBlock.movementPath.GetPoints())
		{
			Debug.Log("Point: " + point.ToString());
		}

		unit.timeline.InsertBlock(moveBlock);
		Debug.Log("Inserted move block 1");

		MoveBlock moveBlock2 = new MoveBlock(unit.id, 2);
		moveBlock2.movementPath.origin = origin;
		moveBlock2.movementPath.target = target;

		unit.timeline.InsertBlock(moveBlock2);
		Debug.Log("Inserted move block 2");

		NetworkPacket packet = new NetworkPacket("test", match);
		Debug.Log("Created packet");

		Debug.Log("JSON: " + packet.ToJSON());
		Debug.Log("Test passed");
	}
}

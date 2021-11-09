using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_LevelLayoutList", menuName = "Level/LayoutList")]
public class LevelLayoutList : ScriptableObject
{
	public List<LevelLayout> layouts;

	public Dictionary<string, LevelLayout> layoutDict = new Dictionary<string, LevelLayout>();
	private bool hasBeenCompiled = false;
	public void Compile()
	{
		layoutDict.Clear();

		foreach (LevelLayout layout in layouts)
		{
			layoutDict.Add(layout.id, layout);
		}


		hasBeenCompiled = true;
	}

	public bool TryGetLevelLayout(string id, out LevelLayout levelLayout)
	{
		levelLayout = null;

		if (hasBeenCompiled)
		{
			Debug.Log("Has compiled!");
			layoutDict.TryGetValue(id, out levelLayout);
		} else
		{
			Debug.Log($"[LevelLayoutList] - List has not been compiled. You need to call LevelLayoutList.Compile() first.");
		}

		return levelLayout != null;
	}
}

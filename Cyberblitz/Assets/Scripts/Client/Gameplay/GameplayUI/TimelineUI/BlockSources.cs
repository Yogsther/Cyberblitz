using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSources : MonoBehaviour
{

	public BlockSource blockSourcePrefab;
	public List<BlockSource> blockSources = new List<BlockSource>();
	TimelineEditor editor;
	public Image field;

	void DeleteButtons()
	{
		foreach (Transform child in transform)
			Destroy(child.gameObject);
	}

	// TODO: Pass the UnitData to see what blocks they have access to.
	public void CreateSourceButtons(TimelineEditor editor)
	{
		this.editor = editor;
		DeleteButtons();
		foreach (KeyValuePair<BlockType, BlockData> data in BlockDataLoader.blockDatas)
		{
			BlockData blockData = data.Value;
			if (blockData.showInToolbar)
			{
				BlockSource blockSource = CreateSourceButton(blockData);
				blockSources.Add(blockSource);
			}
		}
	}

	public void SetVisibility(bool visible)
	{
		foreach (BlockSource blockSource in blockSources)
			blockSource.gameObject.SetActive(visible);
		field.enabled = visible;
	}

	BlockSource CreateSourceButton(BlockData blockData)
	{
		BlockSource blockSource = Instantiate(blockSourcePrefab, transform);
		blockSource.InitiateFromBlockData(blockData, editor);
		return blockSource;
	}

}

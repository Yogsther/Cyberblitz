using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDataLoader : MonoBehaviour
{
    public BlockData[] blockDatasToLoad;

    public readonly static Dictionary<BlockType, BlockData> blockDatas = new Dictionary<BlockType, BlockData>();

    private void Awake()
    {
        foreach (BlockData blockData in blockDatasToLoad)
        {
            blockDatas.Add(blockData.type, blockData);
        }
    }

    public static BlockData GetBlockData(BlockType type)
    {
        if(blockDatas.TryGetValue(type, out BlockData blockData))
        {
            return blockData;
        }

        Debug.LogWarning($"Could not find BlockData of type {type}");

        return null;
    }
}

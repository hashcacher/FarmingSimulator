using UnityEngine;
using System.Collections.Generic;

public class WorldCreator : MonoBehaviour, IBlockEventTarget, IFlowerEvent
{
    // These must be in the same order as the block types prefab in Util
    public GameObject[] blockPrefabs;
    public GameObject player;
    public GameObject sheepPrefab;
    public int nSheep;

    public int waterThreshold;

    [HideInInspector]
    public Block[,,] blocks;
    [HideInInspector]
    public IntVector3 size;

	// Use this for initialization
	void Start ()
    {
        int sX = 100, sY = 100, sZ = 100;
        blocks = new Block[sX, sY, sZ];

        size = new IntVector3(sX, sY, sZ);
        GenerateWorld(size);

        Vector3 spawnPos = new Vector3(size.x / 2, size.y / 4, size.z / 2);
        Instantiate(player, spawnPos, Quaternion.identity);

        SpawnSheep(nSheep);
	}


    private void GenerateWorld(IntVector3 dim)
    {
        for (int x = 0; x < dim.x; x++)
        {
            for (int z = 0; z < dim.z; z++)
            {
                float height =
                    // High frequency noise
                    Mathf.PerlinNoise(5 * x / dim.x, 2f * z / dim.z) * dim.y * .2f + 
                    // Low frequency noise
                    Mathf.PerlinNoise(1 * x / dim.x, 0) * dim.y * .15f // Amplitude coeff
                    ;

                height = Mathf.Min(height, dim.y);

                for (int y = 0; y < height; y++)
                {
                    // Utils.BlockTypes type = GetTypeForHeight(y);
                    CreateBlock(x, y, z, Utils.BlockTypes.Dirt, false);
                }
            }
        }

        // Second pass to instantiate visible blocks + water
        List<IntVector3> activeWaterBlocks = new List<IntVector3>();
        for (int x = 0; x < dim.x; x++)
        {
            for (int z = 0; z < dim.z; z++)
            {
                bool topCreated = false;
                for (int y = 0; y < dim.y; y++)
                {
                    if (blocks[x, y, z].created)
                    {
                        if (!BlockSurrounded(x, y, z))
                        {
                            topCreated = true;
                            CreateBlock(x, y, z, blocks[x, y, z].type, true);
                        }
                    }
                    else if (topCreated && y < waterThreshold)
                    {
                        CreateBlock(x, y, z, Utils.BlockTypes.Water, true);
                        activeWaterBlocks.Add(new IntVector3(x, y, z));
                    }
                    else break;
                }
            }
        }

        WorldUpdater updater = GetComponent<WorldUpdater>();
        if (updater) updater.InitWater(activeWaterBlocks);
    }

    public void MakeDirtGrass(int x, int y, int z)
    {
        if (blocks[x, y, z].created)
        {
            Destroy(blocks[x, y, z].go);
        }

        GameObject go = CreateBlock(x, y, z, Utils.BlockTypes.DirtGrass, true);
        blocks[x, y, z] = new Block(Utils.BlockTypes.DirtGrass, true, go);
    }

    private Utils.BlockTypes GetTypeForHeight(int height)
    {
        if (height < 75) return Utils.BlockTypes.Gravel;
        else if (height < 80) return Utils.BlockTypes.Dirt;
        else if (height < 90) return Utils.BlockTypes.DirtGrass;
        else return Utils.BlockTypes.Grass;
    }

    public GameObject CreateBlock(int x, int y, int z, Utils.BlockTypes type, bool visible)
    {
        if (!Utils.InBounds(new IntVector3(x, y, z), size)) return null;

        // Clean up old block
        if (blocks[x, y, z].go) Destroy(blocks[x, y, z].go);

        GameObject go = null;
        if (visible)
        {
            go = Instantiate(blockPrefabs[(short)type], new Vector3(x, y, z), Quaternion.identity) as GameObject;
            blocks[x, y, z] = new Block(type, visible, go);
        }
        else
        {
            blocks[x, y, z] = new Block(type, visible);
        }

        return go;
    }

    public GameObject CreateBlock(IntVector3 pos, Utils.BlockTypes type, bool visible)
    {
        return CreateBlock(pos.x, pos.y, pos.z, type, visible);
    }

    private void RevealBlock(IntVector3 pos)
    {
        Block block = blocks[pos.x, pos.y, pos.z];
        if (block.created && !block.visible)
        {
            blocks[pos.x,pos.y,pos.z].visible = true;
            blocks[pos.x, pos.y, pos.z].go = 
                Instantiate(blockPrefabs[(short)block.type], pos.ToVector3(), Quaternion.identity) as GameObject;
        }
    }

    private bool BlockSurrounded(int tx, int ty, int tz)
    {
        foreach (IntVector3 intVec in Utils.GetNeighbors(tx, ty, tz))
        {
            if (!Utils.InBounds(intVec, size) || 
                !blocks[intVec.x, intVec.y, intVec.z].created) return false;
        }
        return true;
    }

    public void SpawnSheep(int n)
    {
        for(int i = 0; i < n; i++)
        {
            Vector3 pos = new Vector3(Random.Range(0f, size.x), size.y / 2, Random.Range(0f, size.z));
            Instantiate(sheepPrefab, pos, Quaternion.Euler(0, Random.Range(0,360), 0));
        }
    }

    public void Break(Transform trans)
    {
        IntVector3 arrayPos = new IntVector3(trans.position);

        // Clean up
        Destroy(blocks[arrayPos.x, arrayPos.y, arrayPos.z].go);
        blocks[arrayPos.x, arrayPos.y, arrayPos.z].created = false;

        // Neighbors start out invisible to save space; reveal them.
        foreach (IntVector3 pos in Utils.GetNeighborsInBounds(arrayPos, size))
        {
            RevealBlock(pos);
        }
    }

    public void Build(IntVector3 pos, Utils.BlockTypes type)
    {
        CreateBlock(pos.x, pos.y, pos.z, type, true);
    }

    public void Poop(IntVector3 pos)
    {
        CreateBlock(pos, Utils.BlockTypes.Poop, true);
    }

    public void Flower(IntVector3 pos)
    {
        CreateBlock(pos, Utils.RandomFlower(), true);
    }
}

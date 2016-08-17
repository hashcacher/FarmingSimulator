using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Depends on WorldCreator existing on the same GameObject
class WorldUpdater : MonoBehaviour, IBlockEventTarget
{
    // Blocks like water which need to be checked next update.
    private List<IntVector3> activeWaterBlocks, activeGrassBlocks;
    private List<IntVector3> nextActiveWaterBlocks, nextActiveGrassBlocks;

    private WorldCreator world;

    void Awake()
    {
        world = GetComponent<WorldCreator>();
        activeGrassBlocks = new List<IntVector3>();
        nextActiveGrassBlocks = new List<IntVector3>();
        nextActiveWaterBlocks = new List<IntVector3>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWater();
        UpdateGrass();

        // Clears the lists that were handled this frame
        activeGrassBlocks.Clear();
        activeWaterBlocks.Clear();

        // Sets the active blocks for next frame
        activeGrassBlocks.AddRange(nextActiveGrassBlocks);
        activeWaterBlocks.AddRange(nextActiveWaterBlocks);

        // Clears the lists for next frame
        nextActiveGrassBlocks.Clear();
        nextActiveWaterBlocks.Clear();
    }

    // Makes the water spread and turn dirt into grrass
    private void UpdateWater()
    {
        for (int i = 0; i < activeWaterBlocks.Count; i++)
        {   
            IntVector3 pos = activeWaterBlocks[i];
            foreach (IntVector3 neighborPos in Utils.GetNeighborsInBounds(pos, world.size))
            {
                if (WaterShouldSpread(pos, neighborPos))
                {
                    StartCoroutine(CreateBlockWithDelay(neighborPos, Utils.BlockTypes.Water, true, 1, activeWaterBlocks));
                }
                else if(WaterShouldCreateGrass(pos, neighborPos))
                {
                    StartCoroutine(CreateBlockWithDelay(neighborPos, Utils.BlockTypes.Grass, true, Random.Range(5f, 30f), activeGrassBlocks));
                }
            }
        }
    }

    // Makes the grass spread
    private void UpdateGrass()
    {
        for (int i = 0; i < activeGrassBlocks.Count; i++)
        {
            IntVector3 pos = activeGrassBlocks[i];
            foreach (IntVector3 neighborPos in Utils.GetNeighborsInBounds(pos, world.size))
            {
                if (GrassShouldSpread(pos, neighborPos))
                {
                    StartCoroutine(
                        CreateBlockWithDelay(neighborPos, Utils.BlockTypes.Grass, true, Random.Range(5f, 30f), nextActiveGrassBlocks));
                }
            }
        }
    }

    // Creates blocks after delay
    // Then adds the position to list of active positions
    public IEnumerator CreateBlockWithDelay(IntVector3 pos, Utils.BlockTypes type, bool visible, float delay, List<IntVector3> list)
    {
        yield return new WaitForSeconds(delay);
        world.CreateBlock(pos.x, pos.y, pos.z, type, true);
        world.blocks[pos.x, pos.y, pos.z].type = type;
        yield return new WaitForEndOfFrame();
        list.Add(pos);
    }

    private bool WaterShouldCreateGrass(IntVector3 source, IntVector3 dest)
    {
        if (source.y == dest.y && !world.blocks[dest.x,dest.y+1,dest.z].created)
        {
            Block destBlock = GetBlock(dest);
            if (destBlock.type == Utils.BlockTypes.Dirt) return true; // Check if it's created?
        }

        return false;
    }

    private bool WaterShouldSpread(IntVector3 source, IntVector3 dest)
    {
        if (source.y >= dest.y)
        {
            Block destBlock = GetBlock(dest);
            if (destBlock.type != Utils.BlockTypes.Water && !destBlock.created) return true;
        }

        return false;
    }

    private bool GrassShouldSpread(IntVector3 source, IntVector3 dest)
    {
        if (source.y == dest.y && !world.blocks[dest.x,dest.y+1,dest.z].created)
        {
            Block destBlock = GetBlock(dest);
            if (destBlock.type == Utils.BlockTypes.Dirt) return true; // Created matters?
        }

        return false;
    }


    private Block GetBlock(IntVector3 vec)
    {
        return world.blocks[vec.x, vec.y, vec.z];
    }

    public void Break(Transform trans)
    {
        IntVector3 pos = new IntVector3(trans.position);
        MakeNeighborsActive(pos);
    }

    public void Build(IntVector3 pos, Utils.BlockTypes type)
    {
        MakeNeighborsActive(pos);
    }

    // TODO: give this event its own event interface
    public void Poop(IntVector3 pos)
    {

    }

    private void MakeNeighborsActive(IntVector3 pos)
    {
        foreach (IntVector3 neighborPos in Utils.GetNeighborsInBounds(pos, world.size))
        {
            Block block = world.blocks[neighborPos.x, neighborPos.y, neighborPos.z];
            if (block.type == Utils.BlockTypes.Water) activeWaterBlocks.Add(neighborPos);
            else if (block.type == Utils.BlockTypes.Grass) activeGrassBlocks.Add(neighborPos);
        }
    }    

    public void InitWater(List<IntVector3> water)
    {
        activeWaterBlocks = water;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public static class BlockFactory
    {
        public static Block SpawnBlock(BlockType blockType)
        {
            Block block = new Block(blockType);

            // Set Board
            if (blockType == BlockType.BASIC)
                block.breed = (BlockBreed)UnityEngine.Random.Range(0, 6);
            else if (blockType == BlockType.EMPTY)
                block.breed = BlockBreed.NA;

            return block;
        }
    }
}
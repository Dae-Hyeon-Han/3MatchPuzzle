using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public enum BlockType
    {
        EMPTY = 0,
        BASIC = 1,
    }

    public enum BlockBreed
    {
        NA = -1,
        BREED_0 = 0,
        BREED_1 = 1,
        BREED_2 = 2,
        BREED_3 = 3,
        BREED_4 = 4,
        BREED_5 = 5,
    }

    static class BlockMethod
    {
        public static bool IsSafeEqual(this Block block, Block targetBlock)
        {
            if (block == null)
                return false;

            return block.IsEqual(targetBlock);
        }
    }
}

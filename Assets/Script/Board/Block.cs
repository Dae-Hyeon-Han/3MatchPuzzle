using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public class Block
    {
        protected BlockType m_BlockType;
        public BlockType blockType
        {
            get { return m_BlockType; }
            set { m_BlockType = value; }
        }

        public Block(BlockType blockType)
        {
            m_BlockType = blockType;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public class Cell
    {
        protected CellType m_CellType;
        public CellType cellType
        {
            get { return m_CellType; }
            set { m_CellType = value; }
        }

        public Cell(CellType cellType)
        {
            m_CellType = cellType;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public class Cell
    {
        protected CellType m_CellType;
        protected CellBehaviour m_CellBehaviour;

        public CellType cellType
        {
            get { return m_CellType; }
            set { m_CellType = value; }
        }

        public CellBehaviour cellBehaviour
        {
            get { return m_CellBehaviour; }
            set
            {
                m_CellBehaviour = value;
                m_CellBehaviour.SetCell(this);
            }
        }

        public Cell(CellType cellType)
        {
            m_CellType = cellType;
        }

        public Cell InstantiateCellObj(GameObject cellPrefab, Transform containerObj)
        {
            // Cell 오브젝트 생성
            GameObject newObj = Object.Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            // 컨테이너의 차일드로 Cell을 포함
            newObj.transform.parent = containerObj;

            // Cell 오브젝트에 적용된 CellBehaviour 컴포넌트를 포관
            this.cellBehaviour = newObj.transform.GetComponent<CellBehaviour>();

            return this;
        }

        public void Move(float x, float y)
        {
            cellBehaviour.transform.position = new Vector3(x,y);
        }
    }
}
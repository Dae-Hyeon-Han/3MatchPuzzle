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
            // Cell ������Ʈ ����
            GameObject newObj = Object.Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            // �����̳��� ���ϵ�� Cell�� ����
            newObj.transform.parent = containerObj;

            // Cell ������Ʈ�� ����� CellBehaviour ������Ʈ�� ����
            this.cellBehaviour = newObj.transform.GetComponent<CellBehaviour>();

            return this;
        }

        public void Move(float x, float y)
        {
            cellBehaviour.transform.position = new Vector3(x,y);
        }
    }
}
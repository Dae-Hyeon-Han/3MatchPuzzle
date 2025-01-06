using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public class CellBehaviour : MonoBehaviour
    {
        Cell cell;
        [SerializeField] SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateView(false);
        }

        public void SetCell(Cell cell)
        {
            this.cell = cell;
        }

        public void UpdateView(bool bValueChanged)
        {
            if(cell.cellType == CellType.EMPTY)
            {
                spriteRenderer = null;
            }
        }
    }
}
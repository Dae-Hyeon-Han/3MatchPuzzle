using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Controller
{
    // 블록의 위치를 저장하는 구조체
    public struct BlockPos
    {
        public int row { get; set; }
        public int column { get; set; }

        public BlockPos(int row = 0, int column = 0)
        {
            this.row = row;
            this.column = column;
        }

        public override bool Equals(object obj)
        {
            return obj is BlockPos pos && row == pos.row && column == pos.row;
        }

        public override int GetHashCode()
        {
            var hashCode = -928284752;
            hashCode = hashCode * -1521134295 + row.GetHashCode();
            hashCode = hashCode * -1521134295 + column.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"(row = {row}, column = {column})";
        }
    }
}
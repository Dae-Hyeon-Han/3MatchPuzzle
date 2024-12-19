using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public enum CellType
    {
        EMPTY = 0,          // 빈 공간. 블록이 위치할 수 없음
        BASIC = 1,          // 배경있는 기본형
        FIXTURE = 2,        // 고정된 장애물(변화 없음)
        ICON = 3,           // 그림: 블록이동 가능. 블록 Clear 되면 Basic. 출력: CellBg
    }
}
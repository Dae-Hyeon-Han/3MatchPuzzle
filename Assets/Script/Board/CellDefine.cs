using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public enum CellType
    {
        EMPTY = 0,          // �� ����. ����� ��ġ�� �� ����
        BASIC = 1,          // ����ִ� �⺻��
        FIXTURE = 2,        // ������ ��ֹ�(��ȭ ����)
        ICON = 3,           // �׸�: ����̵� ����. ��� Clear �Ǹ� Basic. ���: CellBg
    }
}
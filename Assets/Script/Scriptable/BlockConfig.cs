using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Scripable
{
    [CreateAssetMenu(menuName = "Puzzle/Block Config", fileName = "BlockConfig.asset")]
    public class BlockConfig : ScriptableObject
    {
        public Sprite[] basicBlockSprites;
    }
}
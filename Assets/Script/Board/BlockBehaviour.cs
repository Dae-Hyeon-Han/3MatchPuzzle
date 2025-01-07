using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Scripable;

namespace Puzzle.Board
{
    public class BlockBehaviour : MonoBehaviour
    {
        Block block;
        SpriteRenderer spriteRenderer;
        [SerializeField] BlockConfig blockConfig;

        // Start is called before the first frame update
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateView(false);
        }

        internal void SetBlock(Block block)
        {
            this.block = block;
        }

        public void UpdateView(bool bValueChanged)
        {
            if(block.blockType == BlockType.EMPTY)
            {
                spriteRenderer.sprite = null;
            }
            else if(block.blockType == BlockType.BASIC)
            {
                //spriteRenderer.sprite = blockConfig.basicBlockSprites[1];
                spriteRenderer.sprite = blockConfig.basicBlockSprites[(int)block.breed];
            }
        }
    }
}
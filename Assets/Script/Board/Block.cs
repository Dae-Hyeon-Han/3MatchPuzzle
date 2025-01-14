using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public class Block
    {
        protected BlockType m_BlockType;
        protected BlockBehaviour m_BlockBehaviour;
        protected BlockBreed m_Breed;
        public Transform blockObj { get { return m_BlockBehaviour?.transform; } }
        Vector2Int m_vtDuplicate;           // 블록 중복 개수. Shuffle시 중복 검사에 사용

        public BlockType blockType
        {
            get { return m_BlockType; }
            set { m_BlockType = value; }
        }

        public BlockBehaviour blockBehaviour
        {
            get { return m_BlockBehaviour; }
            set
            {
                m_BlockBehaviour = value;
                m_BlockBehaviour.SetBlock(this);
            }
        }

        public BlockBreed breed
        {
            get { return m_Breed; }
            set
            {
                m_Breed = value;
                m_BlockBehaviour?.UpdateView(true);
            }
        }

        public Block(BlockType blockType)
        {
            m_BlockType = blockType;
        }

        public int horzDuplicate
        {
            get { return m_vtDuplicate.x; }
            set { m_vtDuplicate.x = value; }
        }

        public int vertDuplicate
        {
            get { return m_vtDuplicate.y; }
            set { m_vtDuplicate.y = value; }
        }

        public void ResetDuplicationInfo()
        {
            m_vtDuplicate.x = 0;
            m_vtDuplicate.y = 0;
        }

        public bool IsEqual(Block target)
        {
            if (IsMatchableBlock() && this.breed == target.breed)
                return true;

            return false;
        }

        public bool IsMatchableBlock()
        {
            return !(blockType == BlockType.EMPTY);
        }

        internal Block InstantiateBlockObj(GameObject blockPrefab, Transform containerObj)
        {
            if (IsValidate() == false)
                return null;

            // 블록 오브젝트 생성
            GameObject newObj = Object.Instantiate(blockPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            // 컨테이너의 차일드로 블록 포함
            newObj.transform.parent = containerObj;

            // 블록 오브젝트에 적용된 BlockBehaviour 컴포넌트를 보관
            this.blockBehaviour = newObj.transform.GetComponent<BlockBehaviour>();

            return this;
        }

        internal void Move(float x, float y)
        {
            blockBehaviour.transform.position = new Vector3(x, y);
        }

        public bool IsValidate()
        {
            return blockType != BlockType.EMPTY;
        }

        public void MoveTo(Vector3 to, float duration)
        {
            blockBehaviour.StartCoroutine(Controller.Action2D.MoveTo(blockObj, to, duration));
        }

        public bool IsSwipeable(Block baseBlock)
        {
            return true;
        }
    }
}

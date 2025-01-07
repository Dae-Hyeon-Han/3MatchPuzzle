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

        

        internal Block InstantiateBlockObj(GameObject blockPrefab, Transform containerObj)
        {
            if (IsValidate() == false)
                return null;

            // ��� ������Ʈ ����
            GameObject newObj = Object.Instantiate(blockPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            // �����̳��� ���ϵ�� ��� ����
            newObj.transform.parent = containerObj;

            // ��� ������Ʈ�� ����� BlockBehaviour ������Ʈ�� ����
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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Stage
{
    public class StageController : MonoBehaviour
    {
        bool init;
        Stage stage;

        [SerializeField] Transform container;
        [SerializeField] GameObject cellPrefab;
        [SerializeField] GameObject blockPrefab;

        // Start is called before the first frame update
        void Start()
        {
            InitStage();
        }

        void InitStage()
        {
            if (init)
                return;

            init = true;

            BuildStage();
            stage.PrintAll();
        }

        void BuildStage()
        {
            // �������� ����
            stage = StageBuilder.BuildStage(nStage : 1);

            // ������ stage ������ �̿��Ͽ� �� ����
            stage.ComposeStage(cellPrefab, blockPrefab, container);
        }
    }
}
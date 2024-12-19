using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Stage
{
    public class StageController : MonoBehaviour
    {
        bool init;
        Stage stage;

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
            stage = StageBuilder.BuildStage(nStage : 0, row : 9, column : 9);
        }
    }
}
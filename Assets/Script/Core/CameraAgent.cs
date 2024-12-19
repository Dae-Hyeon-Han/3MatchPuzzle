using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAgent : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    [SerializeField] float boardUnit;

    // Start is called before the first frame update
    void Start()
    {
        targetCamera.orthographicSize = boardUnit / targetCamera.aspect;            // ī�޶� ���� ���
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

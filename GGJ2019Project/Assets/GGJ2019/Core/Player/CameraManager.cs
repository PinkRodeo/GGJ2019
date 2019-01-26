using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public float cameraDistance
    {
        get
        {
            return _transform.position.z;
        }
    }

    private Transform _transform;

    protected void Awake()
    {
        _transform = this.transform;
    }

    void Start()
    {
        JunkerGameMode.instance.cameraManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

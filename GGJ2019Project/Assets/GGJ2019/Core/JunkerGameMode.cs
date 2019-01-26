using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkerGameMode : MonoBehaviour
{
    public static JunkerGameMode instance;

    public JunkerPlayer player
    {
        get
        {
            return _player;
        }
        set
        {
            if (_player != null)
            {
                Debug.LogError("Trying to assign a new JunkerPlayer when one already existed.");
            }

            _player = value;
        }
    }

    public CameraManager cameraManager
    {
        get
        {
            return _cameraManager;
        }
        set
        {
            if (_cameraManager != null)
            {
                Debug.LogError("Trying to assign a new cameraManager when one already existed.");
            }

            _cameraManager = value;
        }
    }

    private JunkerPlayer _player;
    private CameraManager _cameraManager;


    protected void Awake ()
    {
        instance = this;
    }

    protected void Start ()
    {
        
    }

    protected void Update ()
    {
        
    }
}

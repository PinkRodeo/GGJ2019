using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleTargeter : MonoBehaviour
{

    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        var worldTargetPos = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -JunkerGameMode.instance.cameraManager.cameraDistance));

        DebugExtension.DebugPoint(worldTargetPos, 10f, 0f, false);
        var playerPos = JunkerGameMode.instance.player.rect_transform.position;
        DebugExtension.DebugArrow(playerPos, worldTargetPos - playerPos);


        if (Input.GetMouseButton(0))
        {
            //Debug.Log("Start move");

            var velocity = new Vector2((worldTargetPos - playerPos).x, (worldTargetPos - playerPos).y);
            JunkerGameMode.instance.player.AddVelocity(velocity);
        }
    }
}

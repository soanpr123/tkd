using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgEfff : MonoBehaviour
{
    private GameObject cam;
    private float length;
    private float backgroundStartPos;
    public float speed;
    public float maxMove;

    public float backGroundSpeed = 0.05f;
    private void Start()
    {

        cam = GameObject.Find("playerCam");

        backgroundStartPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        float temp = (cam.transform.position.x * (1 - 0.9f));
        float backgroundDistance = cam.transform.position.x - backgroundStartPos;
        float maxBackgroundYPos = maxMove + cam.transform.position.y; // độ cao tối đa của map
        float targetYPos = cam.transform.position.y + speed;

        if (targetYPos > maxMove)
        {
            targetYPos = maxMove;
        }

        transform.position = new Vector3(cam.transform.position.x, targetYPos, transform.position.z); // di chuyển background


    }

}

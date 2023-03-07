using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class backGroundController : MonoBehaviour
{

    private GameObject cam;
    private float length;
    private float backgroundStartPos;
    public float backGroundH;
    public float backGroundSpeed = 0.5f;
    private void Start()
    {

        cam = GameObject.Find("playerCam");

        backgroundStartPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        float temp = ((cam.transform.position.y + 10f) * (1 - 0.9f));
        float backgroundDistance = cam.transform.position.x - backgroundStartPos;
        float backYDis = cam.transform.position.y - cam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize - backGroundH;
        backYDis *= backGroundSpeed;



        transform.position = new Vector3((backgroundStartPos + backgroundDistance) * 0.9f, backYDis, transform.position.z);



    }

}

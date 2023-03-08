using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgEfff : MonoBehaviour
{
    private GameObject cam;
    private float length;
    private float backgroundStartPos;
    public float speed;
    float camHeight;
    float camWidth;

    public float backGroundSpeed = 0.05f;
   
    private void Start()
    {

        cam = GameObject.Find("playerCam");
       
        Debug.Log(gameObject.GetComponent<SpriteRenderer>().size);
        backgroundStartPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        float temp = (cam.transform.position.x * (1 - 0.9f));
        float backgroundDistance = cam.transform.position.x - backgroundStartPos;
    
        float targetYPos = cam.transform.position.y + speed;

       
        transform.position = new Vector3(cam.transform.position.x, targetYPos, transform.position.z); // di chuyển background


    }

}

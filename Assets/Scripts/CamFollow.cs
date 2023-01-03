using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target;
    public float speed = 0.125f;
    public Vector3 offset;

    private void LateUpdate()
    {
        Vector3 endPos = target.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, endPos, speed * Time.deltaTime);
        transform.position = smoothPos;
        //transform.LookAt(target);
    }

}

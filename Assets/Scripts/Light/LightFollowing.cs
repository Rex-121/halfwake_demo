using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollowing : MonoBehaviour
{
    public Transform target;
    [Range(0.05f, 1f)]
    public float smoothTime = 0.3f;

    private float _velocityX;

    void Update()
    {
        float x = Mathf.SmoothDamp(transform.position.x, target.position.x, ref _velocityX, smoothTime);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}

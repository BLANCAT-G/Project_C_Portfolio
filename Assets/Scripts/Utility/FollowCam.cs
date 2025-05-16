using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam=FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y,0);
    }
}

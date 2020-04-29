using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{

    public GameObject thisObject;
    float lastTime = 0f;
    void FixedUpdate()
    {
        if (Time.time - lastTime > 0.01)
        {
            this.transform.position += new Vector3(0, -0.05f, 0);
            if (this.transform.position.y < -4f)
            {
                Destroy(thisObject);
                lastTime = Time.time;
            }
        }
        
    }
}

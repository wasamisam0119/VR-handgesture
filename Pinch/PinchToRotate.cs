using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchToRotate : MonoBehaviour
{
    public OVRHand ovrhand;
    public GameObject gameobject;
    //public GameObject parent;
    public int releaseTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ovrhand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            if (ovrhand.IsDataHighConfidence)
            {
                //gameobject.transform.SetParent(parent.transform, true);
                gameobject.transform.rotation = Quaternion.RotateTowards(gameobject.transform.rotation,ovrhand.transform.rotation, 1f);
                Debug.Log("pinch");
                releaseTime = 15;
            }
        }
        else
        {
            if (releaseTime >= 0)
            {
                releaseTime--;
                //gameobject.transform.SetParent(null);
            }
        }
    }
}

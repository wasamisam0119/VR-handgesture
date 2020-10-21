using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchToMove : MonoBehaviour
{
    public float moveSpeed = 70;
    public OVRHand ovrhand;
    public GameObject gameobject;
    //public GameObject parent;
    public int releaseTime;
    public Vector3 lastlocation;
    Vector3 positionDiff;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ovrhand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            Vector3 handPos = ovrhand.transform.position;
            if (ovrhand.IsDataHighConfidence)
            {

                if (lastlocation!= handPos)
                {
                    positionDiff = handPos - lastlocation; 
                }
                if (positionDiff.magnitude*100<0.1f)
                {
                    positionDiff = Vector3.zero;
                }
                Vector3 tempPos = positionDiff.normalized * moveSpeed * Time.deltaTime;
                gameobject.transform.position = Vector3.Lerp(gameobject.transform.position,gameobject.transform.position + tempPos, Time.deltaTime);
                lastlocation = handPos;
                //gameobject.transform.SetParent(parent.transform, true);
                releaseTime = 20;
            }
        }
        else {
            if (releaseTime > 0) { 
                releaseTime--;
            //gameobject.transform.SetParent(null);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchToScale : MonoBehaviour
{
    public OVRHand Lovrhand;
    public OVRHand Rovrhand;
    public GameObject Lhand;
    public GameObject Rhand;
    //public GameObject parent;


    public GameObject gameobject;
    
    public int releaseTime;
    float dis;
    float disNew;
    float scale;
    float scaleSpeed = 0.75f;
    
    bool isGrab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Lovrhand.GetFingerIsPinching(OVRHand.HandFinger.Index) & Rovrhand.GetFingerIsPinching(OVRHand.HandFinger.Index))
         {
            if (isGrab)
            {
                disNew = (Lhand.transform.position - Rhand.transform.position).magnitude;
                //scale = scale + disNew / dis;

                if (Lovrhand.IsDataHighConfidence & Rovrhand.IsDataHighConfidence) { 
                    if (disNew-dis>0)
                    {
                        gameobject.transform.localScale = gameobject.transform.localScale + Vector3.one * scaleSpeed* Time.deltaTime ;
                    }
                    else
                    {
                        gameobject.transform.localScale = gameobject.transform.localScale - Vector3.one * scaleSpeed* Time.deltaTime ;
                    }
                    dis = disNew;
                }
   
             }
             else
             {
                 isGrab = true;
                 dis = (Lhand.transform.position - Rhand.transform.position).sqrMagnitude;
                 releaseTime = 20;

             }
         }
         else
         {
             if (releaseTime <= 0)
             {
                 isGrab = false;
             }
             else
             {
                 releaseTime--;
                 disNew = (Lhand.transform.position - Rhand.transform.position).magnitude;
                 //scale = scale + (disNew - dis) * 2f;
                if (disNew-dis>0)
                {
                    gameobject.transform.localScale = gameobject.transform.localScale + Vector3.one * scaleSpeed* Time.deltaTime ;
                }
                else
                {
                    gameobject.transform.localScale = gameobject.transform.localScale - Vector3.one * scaleSpeed* Time.deltaTime ;
                }
                dis = disNew;

             }

         }

    }
    private void OnTriggerStay(Collider other)
    {
       /* if (Rovrhand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            gameobject.transform.SetParent(parent.transform, true);
            Debug.Log("pinch");
            releaseTime = 15;
        }
        else
        {
            releaseTime--;
            if (releaseTime <= 0)
            {
                gameobject.transform.SetParent(null);
            }
        }
        /* if (Lovrhand.GetFingerIsPinching(OVRHand.HandFinger.Index) & Rovrhand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            
            if (isGrab)
             {
                 disNew = (Lhand.transform.position - Rhand.transform.position).sqrMagnitude;
                 scale = disNew / dis;

                     gameobject.transform.localScale = new Vector3(scale, scale, scale) * 0.3f;
                 Debug.Log("grab");
                 releaseTime = 15;
             }
             else
             {
                 isGrab = true;
                 dis = (Lhand.transform.position - Rhand.transform.position).sqrMagnitude;
                 releaseTime = 15;

             }
         }
         else
         {
             releaseTime--;
             if (releaseTime <= 0)
             {
                 isGrab = false;
             }
             else
             {
                 disNew = (Lhand.transform.position - Rhand.transform.position).sqrMagnitude;
                 scale = disNew / dis;

                 gameobject.transform.localScale = new Vector3(scale, scale, scale) * 0.3f;
                 Debug.Log("grab");
             }
         }*/
    }

}


using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
//using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour
{
    public HandMotion myHand;
    public TargerEffects targetEffects;


    public Transform target;
    public Transform controlObject;
    public float moveHorizontal;
    public float moveVertical;
    public float moveSpeed;

    public float targetDistance;
    public float targetRotation;
    public float targetScale;


    public float startTime;
    public float taskTimeElapse;
    public int currentTask;
    public int totalNumtask;
    public Vector3[] distanceTask;
    public Quaternion[] rotationTask;
    public Vector3[] scaleTask;
    //public Transform oriControlObjectTrans;
    public Vector3 orilocation;
    public Quaternion orirotation;
    public Vector3 oriscale;

    public Vector3 target_ori_location;
    public Quaternion target_ori_rotation;
    public Vector3 target_ori_scale;

    float angleDiff;
    int successCount;
    int successThreshold;
    public Text[] taskText;
    // Start is called before the first frame update
    void Start()
    {

        
        //task
        distanceTask = new Vector3[] { new Vector3(1f, 1.51f, 1.5f), new Vector3(0.84f, 1.265f, 1.15f) };
        rotationTask = new Quaternion[] { new Quaternion(0.0f, 0.5f, 0.0f, 0.9f), new Quaternion(-0.2f, -0.3f, -0.1f, 0.9f) };
        scaleTask = new Vector3[] { new Vector3(2f, 2f, 2f), new Vector3(0.45f, 0.45f, 0.45f) };
        currentTask = 0;
        totalNumtask = 6;
        successCount = 0;
        successThreshold = 100;

        orilocation = controlObject.position;
        orirotation = controlObject.rotation;
        oriscale = controlObject.localScale;
        controlObject.localScale += Vector3.one;

        target_ori_location = target.position;
        target_ori_rotation = target.rotation;
        target_ori_scale = target.localScale;

        targetDistance = 0.2f;
        targetRotation = 25.0f;
        targetScale = 0.25f;

        moveSpeed = 2f;
        this.LoadNewTask();
        // List<object> allTask = new List<object> { distanceTask, scaleTask, rotationTask };

    }

    void LoadNewTask()
    {

        if (myHand.currentGesture!=null)
        {
            myHand.currentGesture.setIsIn(false);
            myHand.ClearGesture();

        }
        controlObject.position = orilocation;
        controlObject.rotation = orirotation;
        controlObject.localScale = oriscale;

        if (currentTask < 2)
        {
            target.position = distanceTask[currentTask];
            target.rotation = target_ori_rotation;
            target.localScale = target_ori_scale;
        }

        if (currentTask >= 2 && currentTask < 4)
        {

            target.position = target_ori_location;
            target.rotation = rotationTask[currentTask - 2];
            target.localScale = target_ori_scale;
        }


        if (currentTask >= 4 && currentTask < 6)
        {
            target.position = target_ori_location;
            target.rotation = target_ori_rotation;
            target.localScale = scaleTask[currentTask - 4];
        }
        startTime = Time.time; 
    }
   void Update()
    {
        /*
            Text temp = GameObject.Find(1.ToString()).GetComponent<Text>();
            temp.text = "Location: " + orilocation.ToString();
            temp = GameObject.Find(2.ToString()).GetComponent<Text>();
            temp.text = "Rotation: "+ orirotation.eulerAngles.ToString();
            temp = GameObject.Find(3.ToString()).GetComponent<Text>();
            temp.text = "Scale: "+ oriscale.ToString();
        */

        /*
            Text temp = GameObject.Find(4.ToString()).GetComponent<Text>();
            temp.text = "T location : "+target.position.ToString();
            temp = GameObject.Find(5.ToString()).GetComponent<Text>();
            temp.text = "T rotation: "+ target.rotation.eulerAngles.ToString();
            temp = GameObject.Find(6.ToString()).GetComponent<Text>();
            temp.text ="T scale: " + target.localScale.ToString();
          */  
            Text t = GameObject.Find("cur_task").GetComponent<Text>();
            t.text = "Current Task: "+ currentTask.ToString();

        //oriControlObjectTrans.position = new Vector3(1, 2, 3);
        //Debug.Log("Ori"+ oriControlObjectTrans.position);
        if (currentTask >= 0 && currentTask < 2)
        {
            if (Vector3.Distance(target.position, controlObject.position) < targetDistance)
            {
                successCount += 1;
                targetEffects.changeColor(1);
                //Debug.Log("Success");
            }
            else
            {
                targetEffects.changeColor(0);
            }

        }

        if (currentTask >= 2 && currentTask < 4)
        {
            angleDiff = Quaternion.Angle(controlObject.rotation, target.rotation);
            if (angleDiff <= targetRotation)
            {
                successCount += 1;
                targetEffects.changeColor(1);
                //Debug.Log("Rotation Success");
            }
            else
            {
                targetEffects.changeColor(0);
            }
        }
        if (currentTask >= 4 && currentTask < 6)
        {
            float scaleDiff = Mathf.Abs(controlObject.localScale.x - target.localScale.x);
            
            //Debug.Log(scaleDiff);

            if (scaleDiff <= targetScale)
            {
                targetEffects.changeColor(1);
                successCount += 1;
                //Debug.Log("Success");

            }
            else
            {
                targetEffects.changeColor(0);
            }

        }
        if (successCount > successThreshold && currentTask < 6)
        {



            //Text c = GameObject.Find("yo").GetComponent<Text>();
            //t.text = "Success: "+.ToString();
            successCount = 0;
            ShowTime(currentTask);
            currentTask += 1;
            LoadNewTask();
        }



    }
    void ShowTime(int i)
    {   
        taskTimeElapse = Time.time - startTime;
        int tasknum = i + 1;
        string completeText= "Task   "+tasknum.ToString()+"   time:    "+taskTimeElapse.ToString("0.0");
        taskText[i].text = completeText;


    }
 
}

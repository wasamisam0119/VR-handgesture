using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HandMotion : MonoBehaviour
{
    /// <summary>
    /// 手势检测的脚本
    /// </summary>
    public HandMotionDetector handmotionDetector;
    /// <summary>
    /// 被控制的物体
    /// </summary>
    public Transform target;
    public Transform targetInstance;
    public float scaleSpeed = 10;
    public float moveSpeed = 50;
    public float rotSpeed = 10;
    /// <summary>
    /// 缩放限值
    /// </summary>
    public Vector2 scaleClamp = new Vector2() { x = 0.2f, y = 3 };

    /// <summary>
    /// 移动时的缓存
    /// </summary>
    private Vector3 vecCacha;
    /// <summary>
    /// 控制旋转时 目标的世界坐标缓存
    /// </summary>
    private Vector3 targetPosInWorld;


    private void Awake()
    {
        handMotionDetector.OnInitCallback += () => handMotionDetector.Gesture_Scale.Scale_Bigger.InitCallback(null, HandMotion_Scale_Bigger, null);
        handMotionDetector.OnInitCallback += () => handMotionDetector.Gesture_Scale.Scale_Smaller.InitCallback(null, HandMotion_Scale_Smaller, null);
        handMotionDetector.OnInitCallback += () => handMotionDetector.Gesture_Move.InitCallback(null, HandMotion_Move, null);
        handMotionDetector.OnInitCallback += () => handMotionDetector.Gesture_Rotate.InitCallback(HandMotion_Rotate_onStartCallback, HandMotion_Rotate, HandMotion_Rotate_onEndCallback);
    }

    private void HandMotion_Rotate_onEndCallback(Transform obj)
    {
        targetInstance.SetParent(null);
    }

    private void HandMotion_Rotate_onStartCallback(Transform obj)
    {
        targetPosInWorld = targetInstance.position;
        targetInstance.SetParent(obj);
    }
    private void HandMotion_Rotate(Transform trans)
    {
        targetInstance.localPosition = trans.InverseTransformPoint(targetPosInWorld);
        target.rotation = Quaternion.RotateTowards(target.rotation, targetInstance.rotation, Time.deltaTime * rotSpeed);
    }


    //trans 是手的transform
    //vecCacha是手的上一帧位置
    //先记下手的移动差值，如果小于一定误差范围，则不移动，else 在方向上移动。
    //target是物体，targetInstance是中间件
    private void HandMotion_Move(Transform trans)
    {
        if (vecCacha != trans.position)
        {
            //distance difference
            vecCacha = trans.position - vecCacha;
        }
        if (vecCacha.magnitude * 100 < 0.1f)
        {
            vecCacha = trans.position;
            return;
        }
        Vector3 tempPos = vecCacha.normalized * moveSpeed * Time.deltaTime;
        target.position = Vector3.Lerp(target.position, target.position + tempPos, Time.deltaTime);
        vecCacha = trans.position;
    }

    private void HandMotion_Scale_Smaller(Transform trans)
    {
        target.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
        float temp = Mathf.Clamp(target.localScale.x, scaleClamp.x, scaleClamp.y);
        target.localScale = new Vector3(temp, temp, temp);
    }

    private void HandMotion_Scale_Bigger(Transform trans)
    {
        target.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
        //float temp = Mathf.Clamp(target.localScale.x, scaleClamp.x, scaleClamp.y);
        //target.localScale = new Vector3(temp, temp, temp);
    }
}

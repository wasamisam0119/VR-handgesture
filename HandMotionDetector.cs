using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;
using UnityEngine.UI;
using System;
//using UnityEditor.EventSystems;

public class HandMotionDetector : MonoBehaviour
{
    public OVRSkeleton skeleton;

    /// <summary>
    /// 初始化完成之后调用
    /// </summary>
    public event Action OnInitCallback;

    public Text distText;
    public Text timeText;



    [Header("状态")]
    [Space(20)]
    public MotionState motionState = MotionState.Detecting;

    [Header("放大")]
    [Tooltip("进入状态值")]
    public float ScaleBiggerStart = 5;
    [Tooltip("退出状态值")]
    public float ScaleBiggerEnd = 9;
    [Header("缩小")]
    [Tooltip("进入状态值")]
    public float ScaleSmallerStart = 13;
    [Tooltip("退出状态值")]
    public float ScaleSmallerEnd = 7;

    [Header("移动")]
    [Space(20)]
    [Tooltip("状态触发值")]
    public float radTrigger = 0.75f;
    [Tooltip("状态触发值_辅助判断")]
    public float radGrab = 0.5f;

    [Header("旋转")]
    [Space(20)]
    [Tooltip("状态触发值")]
    public float dotTrigger = 0.2f;
    [Tooltip("状态触发值_辅助判断")]
    public float distGrab = 6;
    public float holdMin = 0.1f;
    public float holdmax = 0.22f;

    public GestureBase currentGesture;
    public Gesture_Scale Gesture_Scale;
    public Gesture_Move Gesture_Move;
    public Gesture_Rotate Gesture_Rotate;
    public bool IsInitialized = false;
    public Text[] texts;
    private void Init()
    {
        Gesture_Scale = new Gesture_Scale(skeleton, ScaleBiggerStart, ScaleBiggerEnd, ScaleSmallerStart, ScaleSmallerEnd);
        Gesture_Move = new Gesture_Move(skeleton, radTrigger, radGrab);
        Gesture_Rotate = new Gesture_Rotate(skeleton, dotTrigger, distGrab, holdMin, holdmax);

        Gesture_Scale.OnInit();
        Gesture_Move.OnInit();
        Gesture_Rotate.OnInit();
        IsInitialized = true;
    }

    void Start()
    {
        StartCoroutine(StartSkeletonInit());
    }


    IEnumerator StartSkeletonInit()
    {
        while (skeleton.IsInitialized == false)
        {
            yield return null;
        }
        Init();
        OnInitCallback?.Invoke();
    }

    internal OVRBone GetOvrBone(OVRSkeleton.BoneId boneId)
    {
        foreach (var item in skeleton.Bones)
        {
            if (item.Id == boneId)
            {
                return new OVRBone(item.Id, item.ParentBoneIndex, item.Transform);
            }
        }
        return default;
    }


    void Update()
    {
        if (!IsInitialized)
        {
            texts[0].text = "等待 IsInitialized";
            return;
        }

        if (currentGesture == null || !skeleton.IsInitialized)
        {
            texts[0].text = "完成 IsInitialized";
            return;
        }
        switch (motionState)
        {
            case MotionState.Detecting:
                if (currentGesture.OnStart())
                {
                    motionState = MotionState.Doing;
                }

                break;
            case MotionState.Doing:

                currentGesture.OnUpdate();

                if (currentGesture.OnEnd())
                {
                    motionState = MotionState.Detecting;

                }
                break;
        }
        texts[0].text = motionState.ToString();
    }


    public void ChangeGestureToMove()
    {
        ClearGesture();
        motionState = MotionState.Detecting;
        texts[6].text = "ChangeGestureToMove";
        currentGesture = Gesture_Move;
        Debug.Log("change to Gesture_Move");
    }
    public void ChangeGestureToRotate()
    {
        ClearGesture();
        motionState = MotionState.Detecting;
        texts[6].text = "ChangeGestureToRotate";
        currentGesture = Gesture_Rotate;
        Debug.Log("change to Gesture_Rotate");
    }
    public void ChangeGestureToScale()
    {
        ClearGesture();
        texts[6].text = "ChangeGestureToScale";
        motionState = MotionState.Detecting;
        currentGesture = Gesture_Scale;
        Debug.Log("change to Gesture_Scale");
    }
    public void ClearGesture()
    {
        currentGesture = null;
    }


}

/// <summary>
/// 接口
/// </summary>
public interface IMotionBase
{
    bool OnInit();
    bool OnStart();
    bool OnUpdate();
    bool OnEnd();

}


/// <summary>
/// 状态枚举
/// </summary>
public enum MotionState
{
    Detecting, Doing
}

//public enum MotionCache

/// <summary>
/// 手势抽象基类
/// </summary>
[Serializable]
public abstract class GestureBase : IMotionBase
{
    internal OVRSkeleton skeleton;
    internal Action<Transform> onUpdateCallback;
    internal Action<Transform> onStartCallback;
    internal Action<Transform> onEndCallback;

    public void setIsIn(bool isIn)
    {
        this.isIn = isIn;
    }
    public void InitCallback(Action<Transform> OnStart, Action<Transform> OnUpdate, Action<Transform> OnEnd)
    {
        onStartCallback = OnStart;
        onUpdateCallback = OnUpdate;
        onEndCallback = OnEnd;
    }


    internal float oriTime = 1;

    internal float cacheTime = 1;

    internal bool isIn = false;


    internal int rightFrameCount = 0;
    internal int FrameCounter = 0;
    internal int OriRightFrameCount = 20;
    internal int endFrameCount= 20;




    public string str;
    public string str1;
    public string str2;
    public string str3;
    public string str4;
    public string str5;
    internal enum Finger
    {
        Thumb, Index, Middle, Ring, Pinky
    }

    internal OVRBone[] thumbBones;
    internal OVRBone[] indexBones;
    internal OVRBone[] middleBones;
    internal OVRBone[] ringBones;
    internal OVRBone[] pinkyBones;


    /// <summary>
    /// 所有指尖骨头ID
    /// </summary>
    OVRSkeleton.BoneId[] boneIds = new OVRSkeleton.BoneId[]
    {
        OVRSkeleton.BoneId.Hand_ThumbTip,
        OVRSkeleton.BoneId.Hand_IndexTip,
        OVRSkeleton.BoneId.Hand_MiddleTip,
        OVRSkeleton.BoneId.Hand_RingTip,
        OVRSkeleton.BoneId.Hand_PinkyTip
    };


    /// <summary>
    /// 获取5根手指头末端的距离总和
    /// </summary>
    /// <returns></returns>
    public float GetFingerTipDist()
    {
        var bones = GetOvrBone(boneIds);
        float fingerTipDist = 0;
        fingerTipDist = SumBonesDist(fingerTipDist, bones);
        return fingerTipDist;
    }


    /// <summary>
    /// 获取手指骨头
    /// </summary>
    /// <param name="boneId"></param>
    /// <returns>单个</returns>
    internal OVRBone GetOvrBone(OVRSkeleton.BoneId boneId)
    {
        //Debug.Log(skeleton == null);
        foreach (var item in skeleton.Bones)
        {
            if (item.Id == boneId)
            {
                return item;
            }
        }
        return default;
    }

    /// <summary>
    /// 获取手指骨头
    /// </summary>
    /// <param name="boneIds"></param>
    /// <returns>数组</returns>
    internal OVRBone[] GetOvrBone(params OVRSkeleton.BoneId[] boneIds)
    {
        OVRBone[] bones = new OVRBone[boneIds.Length];
        for (int i = 0; i < boneIds.Length; i++)
        {
            bones[i] = GetOvrBone(boneIds[i]);
        }
        return bones;
    }


    /// <summary>
    /// 计算几根骨头间的距离
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="oVRBones"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    internal float SumBonesDist(float dist, OVRBone[] oVRBones, int count = 0)
    {
        dist += Vector3.Distance(oVRBones[count].Transform.position, oVRBones[count + 1].Transform.position);
        return ++count >= oVRBones.Length - 1 ? dist : SumBonesDist(dist, oVRBones, count);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="finger"></param>
    /// <param name="radTrigger"></param>
    /// <returns></returns>
    internal bool GetFingerGrab(Finger finger, float radTrigger = 0.5f)
    {
        float dot = 0;
        OVRBone[] tempBones;
        switch (finger)
        {
            case Finger.Thumb:
                tempBones = thumbBones;
                break;
            case Finger.Index:
                tempBones = indexBones;
                break;
            case Finger.Middle:
                tempBones = middleBones;
                break;
            case Finger.Ring:
                tempBones = ringBones;
                break;
            default:
                tempBones = pinkyBones;
                break;
        }
        dot = Vector3.Dot(tempBones[0].Transform.right, tempBones[1].Transform.right);

        Debug.DrawRay(tempBones[0].Transform.position, tempBones[1].Transform.forward * 0.1f, Color.blue);
        Debug.DrawRay(tempBones[0].Transform.position, tempBones[1].Transform.right * 0.1f, Color.red);
        Debug.DrawRay(tempBones[0].Transform.position, tempBones[1].Transform.up * 0.1f, Color.green);
        //Debug.DrawRay(oVRBones2.Transform.position, oVRBones2.Transform.forward * 0.1f, Color.blue);
        //Debug.DrawRay(oVRBones2.Transform.position, oVRBones2.Transform.right * 0.1f, Color.red);
        //Debug.DrawRay(oVRBones2.Transform.position, oVRBones2.Transform.up * 0.1f, Color.green);

        //Debug.Log("dot " + dot + "  <  " + "radTrigger  " + radTrigger);
        if (radTrigger > 0)
        {
            return Mathf.Abs(dot) > Mathf.Abs(radTrigger);
        }
        else
        {
            str1 = dot.ToString();
            //Debug.Log(dot);
            return Mathf.Abs(dot) < Mathf.Abs(radTrigger);
        }
    }

    public abstract bool OnStart();

    public abstract bool OnUpdate();

    public abstract bool OnEnd();

    public abstract bool OnInit();
}



public sealed class Gesture_Scale : GestureBase
{
    private GestureBase currentGesture;
    public Gesture_Scale_Bigger Scale_Bigger;
    public Gesture_Scale_Smaller Scale_Smaller;

    public Gesture_Scale(OVRSkeleton skeleton, float ScaleBiggerStart, float ScaleBiggerEnd, float ScaleSmallerStart, float ScaleSmallerEnd)
    {
        //Debug.Log("gouzao Gesture_Scale Smaller == null :  " + (Smaller == null));

        Scale_Bigger = new Gesture_Scale_Bigger(skeleton, ScaleBiggerStart, ScaleBiggerEnd);
        Scale_Smaller = new Gesture_Scale_Smaller(skeleton, ScaleSmallerStart, ScaleSmallerEnd);
    }

    public override bool OnEnd()
    {
        if (currentGesture.OnEnd())
        {
            currentGesture = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool OnStart()
    {
        if (Scale_Bigger.OnStart())
        {
            //Debug.Log("Scale_Bigger.OnStart");
            currentGesture = Scale_Bigger;
            return true;
        }
        else if (Scale_Smaller.OnStart())
        {
            //Debug.Log("Scale_Smaller.OnStart");
            currentGesture = Scale_Smaller;
            return true;
        }
        else
        {
            //Debug.Log("currentGesture = null;");
            currentGesture = null;
            return false;
        }
    }

    public override bool OnUpdate()
    {
        //str = "fingerTipDist:" + GetFingerTipDist().ToString();
        //str1 = "thumbBones :" + Vector3.Dot(thumbBones[0].Transform.forward, thumbBones[1].Transform.forward);
        //str2 = "indexBones :" + Vector3.Dot(indexBones[0].Transform.forward, indexBones[1].Transform.forward);
        //str3 = "middleBones :" + Vector3.Dot(middleBones[0].Transform.forward, thumbBones[1].Transform.forward);
        //str4 = "ringBones :" + Vector3.Dot(ringBones[0].Transform.forward, thumbBones[1].Transform.forward);
        //str5 = "pinkyBones :" + Vector3.Dot(pinkyBones[0].Transform.forward, thumbBones[1].Transform.forward);
        if (currentGesture != null)
        {
            Debug.Log("currentGesture != null");
            currentGesture.OnUpdate();
            return true;
        }
        Debug.Log("currentGesture == null");
        return false;
    }

    public override bool OnInit()
    {
        Scale_Smaller.OnInit();
        Scale_Bigger.OnInit();
        return true;
    }



        /// <summary>
    /// 放大手势
    /// </summary>
    public sealed class Gesture_Scale_Bigger : GestureBase
    {
        /// <summary>
        /// 距离字段
        /// </summary>
        public float timeCount = 0;
        private float distCurrent, distCache, distTriggerStart, distTriggerEnd;
        public float initDist;
        public bool triggered = false;
        public Gesture_Scale_Bigger(OVRSkeleton skeleton, float distTriggerStart, float distTriggerEnd)
        {
            this.skeleton = skeleton;
            this.distTriggerStart = distTriggerStart;
            this.distTriggerEnd = distTriggerEnd;

        }

        public override bool OnEnd()
        {
            if (!isIn)
            {
                return false;
            }
            float dsit = GetFingerTipDist();
            if (dsit > distTriggerEnd)
            {
                rightFrameCount++;
            }

            if (rightFrameCount > OriRightFrameCount / 2)
            {
                rightFrameCount = 0;
                isIn = false;
                distCache = dsit;
                onEndCallback?.Invoke(skeleton.transform);
                return true;
            }
            return false;
        }

        public override bool OnInit()
        {
            try
            {
                thumbBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3);
                indexBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index3);
                middleBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2);
                ringBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2);
                pinkyBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }
        }

        public override bool OnStart()
        {
            if (isIn)
            {
                return false;
            }
            float dsit= GetFingerTipDist();

            if (dsit < distTriggerStart && !triggered)
            {
                initDist = dsit;
                triggered = true;
                
            }
            rightFrameCount++;
            if(triggered)
            {
                timeCount += Time.deltaTime;
                if (timeCount > 0.25)
                {

                    if (dsit - initDist > 0.03)
                    {
                        isIn = true;
                        timeCount = 0;
                        rightFrameCount = 0;
                        triggered = false;
                        distCache = dsit;
                        onStartCallback?.Invoke(skeleton.transform);
                        return true;
                    }
                    else
                    {
                        triggered = false;
                        timeCount = 0;
                        rightFrameCount = 0;
                    }
                }
            }
            return false;
            
            
            /*
            if (rightFrameCount > OriRightFrameCount / 2)
            {
                rightFrameCount = 0;
                isIn = true;
                distCache = dsit;
                onStartCallback?.Invoke(skeleton.transform);
                return true;
            }
             */
        }

        public override bool OnUpdate()
        {

            //str = "fingerTipDist :" + GetFingerTipDist().ToString();
            //str1 = "thumbBones :" + Vector3.Dot(thumbBones[0].Transform.forward, thumbBones[1].Transform.forward);
            //str2 = "indexBones :" + Vector3.Dot(indexBones[0].Transform.forward, indexBones[1].Transform.forward);
            //str3 = "middleBones :" + Vector3.Dot(middleBones[0].Transform.forward, middleBones[1].Transform.forward);
            //str4 = "ringBones :" + Vector3.Dot(ringBones[0].Transform.forward, ringBones[1].Transform.forward);
            //str5 = "pinkyBones :" + Vector3.Dot(pinkyBones[0].Transform.forward, pinkyBones[1].Transform.forward);

            distCurrent = GetFingerTipDist();

            if (distCurrent <= distTriggerStart)
            {
                distCache = distCurrent;
            }

            if (distCache < distCurrent)
            {
                onUpdateCallback?.Invoke(skeleton.transform);
                distCache = distCurrent;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 缩小手势
    /// </summary>
    public sealed class Gesture_Scale_Smaller : GestureBase, IMotionBase
    {
        /// <summary>
        /// 距离字段
        /// </summary>
        private float distCurrent, distCache, distTriggerStart, distTriggerEnd;
        public Gesture_Scale_Smaller(OVRSkeleton skeleton, float distTriggerStart, float distTriggerEnd)
        {
            this.skeleton = skeleton;
            this.distTriggerStart = distTriggerStart;
            this.distTriggerEnd = distTriggerEnd;

        }

        public override bool OnEnd()
        {
            if (!isIn)
            {
                return false;
            }
            float dsit = GetFingerTipDist();
            if (dsit < distTriggerEnd)
            {
                rightFrameCount++;
            }

            if (rightFrameCount > OriRightFrameCount / 2)
            {
                rightFrameCount = 0;
                isIn = false;
                distCache = dsit;
                onEndCallback?.Invoke(skeleton.transform);
                return true;
            }
            return false;

        }

        public override bool OnInit()
        {
            try
            {
                thumbBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3);
                indexBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index3);
                middleBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2);
                ringBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2);
                pinkyBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }
        }

        public override bool OnStart()
        {
            if (isIn)
            {
                return false;
            }
            float dsit = GetFingerTipDist();
            if (dsit >= distTriggerStart)
            {
                rightFrameCount++;
            }

            if (rightFrameCount > OriRightFrameCount / 2)
            {
                rightFrameCount = 0;
                isIn = true;
                distCache = dsit;
                onStartCallback?.Invoke(skeleton.transform);
                return true;
            }
            return false;
        }

        public override bool OnUpdate()
        {
            distCurrent = GetFingerTipDist();

            if (distCurrent >= distTriggerStart)
            {
                distCache = distCurrent;
            }

            //Debug.Log("Gesture_Scale_Smaller  distCache " + distCache + " > " + distCurrent + "  bool " + (distCache > distCurrent));

            if (distCache > distCurrent)
            {
                Debug.Log("doing == null " + (onUpdateCallback == null));
                onUpdateCallback?.Invoke(skeleton.transform);
                distCache = distCurrent;
                return true;
            }
            else
            {
                Debug.Log("not doing");
                return false;
            }
        }
    }

}








/// <summary>
/// 移动手势
/// </summary>
public sealed class Gesture_Move : GestureBase, IMotionBase
{
    /// <summary>
    /// 弧度值
    /// </summary>
    private float radTrigger, radGrab;

    public Gesture_Move(OVRSkeleton skeleton, float radTrigger = 0.75f, float radGrab = 0.5f)
    {
        this.skeleton = skeleton;
        this.radTrigger = radTrigger;
        this.radGrab = radGrab;
    }

    private bool GestureTrigger
    {
        get
        {
            //str2 = " GetFingerTipDist  " + GetFingerTipDist() + "  radTrigger  " + radTrigger;

            //str2 = "Finger.Thumb :" + GetFingerGrab(Finger.Thumb, -radGrab)
            //  + "Finger.Index :" + GetFingerGrab(Finger.Index, -radGrab)
            //  + "Finger.Middle :" + GetFingerGrab(Finger.Middle, -radGrab);
            //return GetFingerGrab(Finger.Thumb, -radGrab)
            //    && GetFingerGrab(Finger.Index, -radTrigger)
            //    && GetFingerGrab(Finger.Middle, -radGrab)
            //&& GetFingerGrab(Finger.Ring, -radGrab)
            //&& GetFingerGrab(Finger.Pinky, -radGrab)


            return GetFingerTipDist() < radTrigger;
        }
    }

    private bool GestureTriggerHold
    {
        get
        {
            //    + "Finger.Index :" + GetFingerGrab(Finger.Index, -radGrab)
            //    + "Finger.Middle :" + GetFingerGrab(Finger.Middle, -radGrab);
            //return GetFingerGrab(Finger.Thumb, -radGrab)
            //    && GetFingerGrab(Finger.Index, radTrigger)
            //    && GetFingerGrab(Finger.Middle, -radGrab)
            //&& GetFingerGrab(Finger.Ring, -radGrab)
            //&& GetFingerGrab(Finger.Pinky, -radGrab)
            return GetFingerTipDist() > radTrigger;
            ;
        }
    }


    public override bool OnEnd()
    {

        if (!isIn)
        {
            return false;
        }
        if (GestureTrigger)
        {
            rightFrameCount++;
        }

        if (rightFrameCount > OriRightFrameCount / 2)
        {
            rightFrameCount = 0;
            isIn = false;
            onEndCallback?.Invoke(skeleton.transform);
            return true;
        }
        return false;

    }

    public override bool OnInit()
    {
        try
        {
            thumbBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3);
            indexBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index3);
            middleBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2);
            ringBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2);
            pinkyBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    public override bool OnStart()
    {
        if (isIn)
        {
            return false;
        }
        if (GestureTrigger)
        {
            rightFrameCount++;
        }
        if (rightFrameCount > OriRightFrameCount / 2)
        {
            rightFrameCount = 0;
            isIn = true;
            onStartCallback?.Invoke(skeleton.transform);
            return true;
        }
        return false;
    }

    public override bool OnUpdate()
    {
        str = "GestureTriggerHold " + GestureTriggerHold;
        //str1 = "thumbBones :" + Vector3.Dot(thumbBones[0].Transform.forward, thumbBones[1].Transform.forward);
        //str2 = "indexBones :" + Vector3.Dot(indexBones[0].Transform.forward, indexBones[1].Transform.forward);
        //str3 = "middleBones :" + Vector3.Dot(middleBones[0].Transform.forward, thumbBones[1].Transform.forward);
        //str4 = "ringBones :" + Vector3.Dot(ringBones[0].Transform.forward, thumbBones[1].Transform.forward);
        //str5 = "pinkyBones :" + Vector3.Dot(pinkyBones[0].Transform.forward, thumbBones[1].Transform.forward);

        if (GestureTriggerHold)
        {
            onUpdateCallback?.Invoke(GetOvrBone(OVRSkeleton.BoneId.Hand_Start).Transform);
        }
        return GestureTriggerHold;
    }

}

/// <summary>
/// 旋转手势
/// </summary>
public sealed class Gesture_Rotate : GestureBase, IMotionBase
{
    /// <summary>
    /// 弧度值
    /// </summary>
    private float dotTrigger, distGrab, distHoldMin, distHoldMax;
    private Quaternion currentAngle;
    private Quaternion lastAngle;

    private Transform rotateRoot;

    public Gesture_Rotate(OVRSkeleton skeleton, float dotTrigger = 0.2f, float distGrab = 6f, float distHoldMin = 9, float distHoldMax = 13)
    {
        this.skeleton = skeleton;
        this.dotTrigger = dotTrigger;
        this.distGrab = distGrab;
        this.distHoldMin = distHoldMin;
        this.distHoldMax = distHoldMax;
    }

    private bool GestureTrigger
    {
        get
        {
            return GetFingerGrab(Finger.Thumb, -dotTrigger)
            && GetFingerGrab(Finger.Index, -dotTrigger)
            && GetFingerGrab(Finger.Middle, -dotTrigger)
            //&& GetFingerGrab(Finger.Ring, -dotTrigger) &&
            //GetFingerGrab(Finger.Pinky, -dotTrigger)
            ;
        }
    }


    public override bool OnEnd()
    {
        if (!isIn)
        {
            return false;
        }
        if (GetFingerTipDist() < distGrab && GestureTrigger)
        {
            rightFrameCount++;
        }

        if (rightFrameCount > endFrameCount/2)
        {
            rightFrameCount = 0;
            isIn = false;
            onEndCallback?.Invoke(rotateRoot);
            return true;
        }
        return false;
    }

    public override bool OnInit()
    {
        try
        {
            thumbBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Thumb1, OVRSkeleton.BoneId.Hand_Thumb3);
            indexBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2);
            middleBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2);
            ringBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2);
            pinkyBones = GetOvrBone(OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Hand_Pinky2);
            rotateRoot = GetOvrBone(OVRSkeleton.BoneId.Hand_WristRoot).Transform;



            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    public override bool OnStart()
    {
        if (isIn)
        {
            return false;
        }

        //Debug.Log(" GetFingerTipDist() < distGrab :" + GetFingerTipDist() + "   <   " + distGrab + " && " + " GestureTrigger ==" + GestureTrigger);

        if (GetFingerTipDist() < distGrab && GestureTrigger)
        {
            rightFrameCount++;
        }

        if (rightFrameCount > OriRightFrameCount)
        {
            rightFrameCount = 0;
            onStartCallback?.Invoke(rotateRoot);
            isIn = true;
            return true;
        }
        return false;


    }

    public override bool OnUpdate()
    {
        //float dist = GetFingerTipDist();
        //str2 = +distHoldMin + " " + dist + " " + distHoldMax;
        //if (/*dist < distHoldMax &&*/ dist > distHoldMin)
        //{
        //str3 = "Roting";
        currentAngle = rotateRoot.rotation;
        if( Mathf.Abs(Quaternion.Angle(currentAngle,lastAngle))>1)
        {

            lastAngle = currentAngle;
            onUpdateCallback?.Invoke(rotateRoot);
        }
        return true;
        //}
        //else
        //{
        //    str3 = "Roted";
        //    return false;
        //}
    }

}




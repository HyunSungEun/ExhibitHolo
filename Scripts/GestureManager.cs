using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.UI;

using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

public class GestureManager : QRTracking.Singleton<GestureManager>
{
    //안됨 추후 개발 필요
    enum PointingState
    {
        POINTING_NOWHERE,
        POINTING_SAMEPIPE,
        POINTING_OTHERPIPE,
        MAX
    }
    MixedRealityPose pose;
    RaycastHit searchHit;
    PointingState pointingState = PointingState.POINTING_NOWHERE;

    float trackCompleteTime = 5f;
    float lastTrackFinishedTime = 3f;
    float TrackingTime = 2f;

    float trackRestTime = 6f;
    [SerializeField]
    Image progress;

    Rect camMiddle = new Rect((float)Screen.width / 4f, 0f, (float)Screen.width / 2f, (float)Screen.height);


    // Start is called before the first frame update
    void Start()
    {
        searchHit = new RaycastHit();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastTrackFinishedTime < trackRestTime)
        {
            TrackingTime = 0f;
            progress.fillAmount = TrackingTime / trackCompleteTime;
            return;
        }
        RaycastHit hitted;
        if(false==IsPointingPipe(out hitted))
        {
            TrackingTime -= Time.deltaTime;
            if (TrackingTime < 0f) TrackingTime = 0f;
            progress.fillAmount = TrackingTime / trackCompleteTime;
            return;
        }

        switch(pointingState)
        {
            case PointingState.POINTING_SAMEPIPE:
                TrackingTime += Time.deltaTime;
                if (TrackingTime > trackCompleteTime)
                {
                    lastTrackFinishedTime = Time.time;
                    TrackingTime = trackCompleteTime;
                    searchHit = new RaycastHit();
                    TrackFinished();
                } 
                progress.fillAmount = TrackingTime / trackCompleteTime;
                break;

            case PointingState.POINTING_OTHERPIPE:
                TrackingTime = 0f;
                searchHit = hitted;
                break;
        }
    }
    bool IsPointingPipe( out RaycastHit hitted)
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out pose))
        {
            RaycastHit hit;
            Vector3 ScreenPoint = Camera.main.WorldToScreenPoint(pose.Position);
            Ray indexRay = Camera.main.ScreenPointToRay(ScreenPoint);
            if (camMiddle.Contains(new Vector2(ScreenPoint.x,ScreenPoint.y))  &&       Physics.Raycast(indexRay, out hit, LayerMask.GetMask("PIPE")))
            {
                if(searchHit.collider!=null && searchHit.collider.gameObject ==hit.collider.gameObject)
                {
                    //같은 파이프를 가르키고 있음
                  hitted = searchHit;
                    pointingState = PointingState.POINTING_SAMEPIPE;
                    return true;
                }
                else
                {
                    //다른 파이프를 가르키고 있음
                  hitted = hit;
                    pointingState = PointingState.POINTING_OTHERPIPE;
                    return true;
                }
            }
            else //파이프를 가르키고 있지 않음
            {
                hitted = new RaycastHit();
                pointingState = PointingState.POINTING_NOWHERE;
                return false;
            }
        }
        else //pose를 못 얻어옮
        {
            hitted = new RaycastHit();
            pointingState = PointingState.POINTING_NOWHERE;
            return false;
        }
    }

    void TrackFinished()
    {
        Debug.Log("피니쉬");
        GameObject trackedPipe = searchHit.collider.gameObject;
        if(trackedPipe == null || trackedPipe.GetComponent<Pipe_Info>()==null)
        {
            Debug.Log("MYLOG: 제스쳐매니저_트랙대상 Null");
            return;
        }
        Pipe_Info pipeInfo = trackedPipe.GetComponent<Pipe_Info>();


    }
}

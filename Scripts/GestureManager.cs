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
    //�ȵ� ���� ���� �ʿ�
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
                    //���� �������� ����Ű�� ����
                  hitted = searchHit;
                    pointingState = PointingState.POINTING_SAMEPIPE;
                    return true;
                }
                else
                {
                    //�ٸ� �������� ����Ű�� ����
                  hitted = hit;
                    pointingState = PointingState.POINTING_OTHERPIPE;
                    return true;
                }
            }
            else //�������� ����Ű�� ���� ����
            {
                hitted = new RaycastHit();
                pointingState = PointingState.POINTING_NOWHERE;
                return false;
            }
        }
        else //pose�� �� ����
        {
            hitted = new RaycastHit();
            pointingState = PointingState.POINTING_NOWHERE;
            return false;
        }
    }

    void TrackFinished()
    {
        Debug.Log("�ǴϽ�");
        GameObject trackedPipe = searchHit.collider.gameObject;
        if(trackedPipe == null || trackedPipe.GetComponent<Pipe_Info>()==null)
        {
            Debug.Log("MYLOG: �����ĸŴ���_Ʈ����� Null");
            return;
        }
        Pipe_Info pipeInfo = trackedPipe.GetComponent<Pipe_Info>();


    }
}

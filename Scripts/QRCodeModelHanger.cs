using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QRCodeModelHanger : MonoBehaviour
{
    float trackCompleteTime = 0.5f;
    float lastTrackFinishedTime = 0f;
    float TrackingTime = 0f;

    float trackRestTime=0.8f;
    [SerializeField]
    Image progress;

    private void Start()
    {
        Debug.LogFormat(" 스크린 w{0} h{1}", Screen.width, Screen.height);
    }


    private void Update()
    {
        //트래킹 RestTime
        if (Time.time - lastTrackFinishedTime < trackRestTime)
        {
            TrackingTime = 0f;
            progress.fillAmount = TrackingTime / trackCompleteTime;
            return;
        }
        //지정 스크린 좌표 내에 위치했을 때 트래킹 다이얼
        if(isInCamMiddle())
        {
            TrackingTime+= Time.deltaTime;
            if(TrackingTime  > trackCompleteTime )
            {
                lastTrackFinishedTime = Time.time;
                TrackingTime = trackCompleteTime;
                progress.fillAmount = TrackingTime / trackCompleteTime;
                DeployModel();
            }
        }
        else
        {
            TrackingTime -= Time.deltaTime;
            if (TrackingTime < 0f) TrackingTime = 0f;
        }

        progress.fillAmount = TrackingTime/ trackCompleteTime;
    }
    bool isInCamMiddle()
    {
        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
        Rect camMiddle = new Rect((float)Screen.width / 4f, 0f, (float)Screen.width / 2f, (float)Screen.height );
        return camMiddle.Contains(new Vector2(sp.x, sp.y));
    }

    /// <summary>
    /// QR위치에 모델링 위치
    /// </summary>
    void DeployModel()
    {
        DeployModelType modelType = DeployManager.Instance.GetModelTypeByQRData(GetComponent<QRTracking.QRCode>().CodeText);
        if (modelType == DeployModelType.SMART_PIPE_ALL)
        {
            //모음 QR일 때, 개별 모델링 감춘 후 생성
            DeployManager.Instance.OnQRCodePrefabChanged(DeployModelType.SMART_PIPE_USU,
                                   QRTracking.QRCodesVisualizer.ActionData.Type.Removed,
                                    null);
            DeployManager.Instance.OnQRCodePrefabChanged(DeployModelType.SMART_PIPE_SANGSU,
                                   QRTracking.QRCodesVisualizer.ActionData.Type.Removed,
                                    null);
            DeployManager.Instance.OnQRCodePrefabChanged(DeployModelType.SMART_PIPE_ELEC,
                                   QRTracking.QRCodesVisualizer.ActionData.Type.Removed,
                                    null);

            
            DeployManager.Instance.OnQRCodePrefabChanged(DeployModelType.SMART_PIPE_ALL,
                                  QRTracking.QRCodesVisualizer.ActionData.Type.Removed,
                                   null);

            DeployManager.Instance.OnQRCodePrefabChanged(DeployModelType.SMART_PIPE_ALL,
                                  QRTracking.QRCodesVisualizer.ActionData.Type.Updated,
                                    GetComponent<QRTracking.QRCode>().QRDeployPoint);
        }
        else
        {
            //개별 QR일 때, 전체 QR 모델링에서 해당 파트 감춤
            DeployManager.Instance.TurnOffSmartPipeAllPart(modelType);

            DeployManager.Instance.OnQRCodePrefabChanged(modelType,
                                   QRTracking.QRCodesVisualizer.ActionData.Type.Updated,
                                     GetComponent<QRTracking.QRCode>().QRDeployPoint);
        }
    }

}

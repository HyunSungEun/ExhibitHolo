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
        Debug.LogFormat(" ��ũ�� w{0} h{1}", Screen.width, Screen.height);
    }


    private void Update()
    {
        //Ʈ��ŷ RestTime
        if (Time.time - lastTrackFinishedTime < trackRestTime)
        {
            TrackingTime = 0f;
            progress.fillAmount = TrackingTime / trackCompleteTime;
            return;
        }
        //���� ��ũ�� ��ǥ ���� ��ġ���� �� Ʈ��ŷ ���̾�
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
    /// QR��ġ�� �𵨸� ��ġ
    /// </summary>
    void DeployModel()
    {
        DeployModelType modelType = DeployManager.Instance.GetModelTypeByQRData(GetComponent<QRTracking.QRCode>().CodeText);
        if (modelType == DeployModelType.SMART_PIPE_ALL)
        {
            //���� QR�� ��, ���� �𵨸� ���� �� ����
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
            //���� QR�� ��, ��ü QR �𵨸����� �ش� ��Ʈ ����
            DeployManager.Instance.TurnOffSmartPipeAllPart(modelType);

            DeployManager.Instance.OnQRCodePrefabChanged(modelType,
                                   QRTracking.QRCodesVisualizer.ActionData.Type.Updated,
                                     GetComponent<QRTracking.QRCode>().QRDeployPoint);
        }
    }

}

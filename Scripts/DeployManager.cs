using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployManager : QRTracking.Singleton<DeployManager>
{
    //Ʈ��ŷ threshold
    static readonly float distanceChangeThreshold = 0.03f;
    static readonly float rotationChangeThreshold = 3f;
    static readonly float timeChangeThreshold = 0.5f;

    [SerializeField]
    GameObject[] ModelPrefabs;
    Dictionary<DeployModelType, DeployedModelQR> deployModelQRDict;
    struct DeployedModelQR
    {
        GameObject deployModel;
        public GameObject DeployModel { get { return deployModel; } }
        DeployModelType modelType;
        Vector3 formerPos;
        Quaternion formerRot;
        float lastChangedTime;

        public DeployedModelQR(DeployModelType modelType, GameObject model)
        {
            this.modelType = modelType;
            this.deployModel = model;
            formerPos = Vector3.positiveInfinity;
            formerRot = Quaternion.identity;
            lastChangedTime = 0f;
        }

        bool IsChangeOverThreshold(Transform newTr)
        {
            if (Time.time - lastChangedTime < timeChangeThreshold) return false;
            return Quaternion.Angle(formerRot, newTr.rotation) > rotationChangeThreshold || Vector3.Distance(formerPos,newTr.position)>distanceChangeThreshold;
        }
        /// <summary>
        /// QR ���¿� ���� �𵨸� ��ġ ����
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="newTr"></param>
        public void OriginQRChanged(QRTracking.QRCodesVisualizer.ActionData.Type actionType, Transform newTr)
        {
            ModelDeploy modelDeploy = deployModel.GetComponent<ModelDeploy>();
            switch(actionType)
            {
                case QRTracking.QRCodesVisualizer.ActionData.Type.Added:
                  
                    deployModel.SetActive(true);
                    modelDeploy.Deploy(newTr);
                    formerPos = newTr.position;
                    formerRot = newTr.rotation;
                    lastChangedTime = Time.time;
                  
                    break;
                  
                case QRTracking.QRCodesVisualizer.ActionData.Type.Updated:
                    if (null==newTr ||  !IsChangeOverThreshold(newTr)) return;
                    deployModel.SetActive(true);
                    modelDeploy.Deploy(newTr);
                    formerPos = newTr.position;
                    formerRot = newTr.rotation;
                    lastChangedTime = Time.time;
                    break;
                case QRTracking.QRCodesVisualizer.ActionData.Type.Removed:
                    deployModel.SetActive(false);
                    formerPos = Vector3.positiveInfinity;
                    formerRot = Quaternion.identity;
                    lastChangedTime = 0f;
                    break;
            }
        }
    }
    

    private void Start()
    {
        deployModelQRDict = new Dictionary<DeployModelType, DeployedModelQR>();
        for(int i=0;i< ModelPrefabs.Length;i++)
        {
            GameObject temp = Instantiate<GameObject>(ModelPrefabs[i]);
            deployModelQRDict.Add((DeployModelType)i, new DeployedModelQR((DeployModelType)i,temp));
            temp.SetActive(false);
        }

    }

    public void OnQRCodePrefabChanged(DeployModelType modelType,QRTracking.QRCodesVisualizer.ActionData.Type actionType,Transform changedTr)
    {
        Debug.LogFormat("ü���� {0} , {1} , {2}", actionType.ToString(), 
            (changedTr==null)?"null":
            changedTr.position.ToString() , modelType.ToString());
        deployModelQRDict[modelType].OriginQRChanged(actionType,changedTr);
    }
    /// <summary>
    /// ������ ���� ������ �κ� ����
    /// </summary>
    /// <param name="targetModelType"></param>
    public void TurnOffSmartPipeAllPart(DeployModelType targetModelType)
    {
        if(targetModelType == DeployModelType.SMART_PIPE_ALL ||  !deployModelQRDict[DeployModelType.SMART_PIPE_ALL].DeployModel.activeSelf)
        {
            return;
        }
        SmartPipe_All sma = deployModelQRDict[DeployModelType.SMART_PIPE_ALL].DeployModel.GetComponent<SmartPipe_All>();
        if (!sma) Debug.Log("MYLOG: SmartPipe_All ������Ʈ ����");
        sma.DisablePart(targetModelType);
    }
    /// <summary>
    /// �� ��Ŀ�� (Ȧ�η��� Ȩ) ����� �𵨸� ����
    /// </summary>
    /// <param name="focus"></param>
    private void OnApplicationFocus(bool focus)
    {
        if (deployModelQRDict==null||deployModelQRDict.Count != ModelPrefabs.Length) return;

        for (int i = 0; i < ModelPrefabs.Length; i++)
        {
            deployModelQRDict[(DeployModelType)i].OriginQRChanged(QRTracking.QRCodesVisualizer.ActionData.Type.Removed, null);
        }
    }





    public DeployModelType GetModelTypeByQRData(string QRData)
    {
        //�������� string.Contains   QRüũ
        DeployModelType result = DeployModelType.SMART_PIPE_SANGSU;

         if(QRData.Contains("water"))
        {
            result = DeployModelType.SMART_PIPE_SANGSU;
            return result;
        }
        if (QRData.Contains("rain"))
        {
            result = DeployModelType.SMART_PIPE_USU;
            return result;
        }
        if (QRData.Contains("Electric"))
        {
            result = DeployModelType.SMART_PIPE_ELEC;
            return result;
        }
        if (QRData.Contains("Unit_6"))
        {
            result = DeployModelType.SMART_PIPE_ALL;
            return result;
        }

        return result;

    }
}

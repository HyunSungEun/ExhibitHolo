using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartPipe_All : MonoBehaviour
{
    [SerializeField]
    GameObject[] parts;
    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (GameObject g in parts) g.SetActive(true);
    }
    public void DisablePart(DeployModelType modelType)
    {
        if (modelType==DeployModelType.SMART_PIPE_ALL || gameObject.activeSelf == false) return;
        parts[(int)modelType].SetActive(false);
        bool allDisabled = false;
        foreach(GameObject g in parts)
        {
            allDisabled = allDisabled || g.activeSelf;
        }
        if(!allDisabled)
        {
            DeployManager.Instance.OnQRCodePrefabChanged(DeployModelType.SMART_PIPE_ALL, QRTracking.QRCodesVisualizer.ActionData.Type.Removed, null);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDeploy : MonoBehaviour
{
    [SerializeField]
    Transform originParent;
    [SerializeField]
    Transform deployPointsParent;
    [SerializeField]
    Transform[] deployPoints;
    private void Start()
    {
        originParent = transform.parent;
    }

    public Transform tempChoiced;
    [ContextMenu("aasd")]
    public void debugDeploy() { Deploy(tempChoiced); }

    /// <summary>
    /// 프리팹에 지정된 트랜스폼 위치를 인자로 받은  QR코드 트랜스폼과 일치시켜 그 위치에 포지셔닝
    /// </summary>
    /// <param name="choicedDeployPoint"></param>
    public void Deploy(Transform choicedDeployPoint)
    {
        if (deployPoints == null || deployPoints.Length == 0) return;
        GameObject deployPoint = deployPoints[0].gameObject;
        deployPoint.transform.parent = null;
        gameObject.transform.parent = deployPoint.transform;
        /*
        Vector3 rotV = new Vector3(0f, choicedDeployPoint.rotation.eulerAngles.y, 180f);//yaw만 적용
        deployPoint.transform.SetPositionAndRotation(choicedDeployPoint.position, Quaternion.Euler(rotV));
        */
        deployPoint.transform.SetPositionAndRotation(choicedDeployPoint.position, choicedDeployPoint.rotation);


        deployPoint.transform.parent = deployPointsParent;
        gameObject.transform.parent = originParent;
    }
}

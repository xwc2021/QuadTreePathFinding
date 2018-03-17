using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMaker : MonoBehaviour {

    public BoxColliderMetaInfo[] boxColliderMetaInfoList;

    //分割的臨界值
    [SerializeField]
    float smallestX=1;

    [SerializeField]
    float smallestZ=1;

    [SerializeField]
    Transform border;

    [SerializeField]
    int cellCountX = 10;

    [SerializeField]
    int cellCountZ = 10;

    public int GetCellCountX() { return cellCountX; }
    public int GetCellCountZ() { return cellCountZ; }

    public Vector3 GetDirX() { return transform.right; }
    public Vector3 GetDirZ() { return transform.forward; }

    public Vector3 GetOrigin() { return transform.position; }
    public Vector3 GetBorder() { return border.position; }

    public void GetAllBoxColliderMetaInfoInSceneAndGenerateRect()
    {
        boxColliderMetaInfoList=GameObject.FindObjectsOfType<BoxColliderMetaInfo>();

        foreach (var boxColliderMetaInfo in boxColliderMetaInfoList)
        {
            var boxCollider = boxColliderMetaInfo.GetComponent<BoxCollider>();
            boxColliderMetaInfo.boxCollider = boxCollider;
            boxColliderMetaInfo.GenerateRectInfo();
        }
    }

    //除了是四叉樹，所有葉節點彼此還會相連
    public void GenerateQuadTreeConnectedNode()
    {
    }

    public Transform P0;
    public Transform P1;
    public Transform P2;
    public Transform P3;
    public Transform C1;
    public Transform C2;

    public void TestIsIntersect()
    {
        Vector3 p0 = P0.position;
        Vector3 p1 = P1.position;
        Vector3 p2 = P2.position;
        Vector3 p3 = P3.position;
        bool result = BoxColliderMetaInfo.IsIntersect(p0, p1, p2, p3,C1,C2);
        Debug.Log(result);
    }
}

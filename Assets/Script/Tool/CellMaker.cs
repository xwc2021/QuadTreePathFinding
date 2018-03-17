using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMaker : MonoBehaviour {

    public BoxColliderMetaInfo[] boxColliderMetaInfoList;

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

    public void GetAllBoxColliderInScene()
    {
        boxColliderMetaInfoList=GameObject.FindObjectsOfType<BoxColliderMetaInfo>();

        foreach (var boxColliderMetaInfo in boxColliderMetaInfoList)
        {
            var boxCollider = boxColliderMetaInfo.GetComponent<BoxCollider>();
            boxColliderMetaInfo.boxCollider = boxCollider;
            boxColliderMetaInfo.GenerateRectInfo();
        }
    }
}

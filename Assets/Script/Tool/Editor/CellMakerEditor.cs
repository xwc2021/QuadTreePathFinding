using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CellMaker))]
public class CellMakerEditor : Editor {

    CellMaker cellMaker;
    void OnEnable()
    {
        cellMaker = (CellMaker)target;
    }

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        var OuterQuadRectCount = cellMaker.GetOuterQuadRect().Count;
        GUILayout.Label("OuterQuadRectCount:"+OuterQuadRectCount);

        if (GUILayout.Button("Get All BoxCollider & Generate QuadTree"))
        {
            cellMaker.GetAllBoxColliderMetaInfoInSceneAndGenerateRect();

            cellMaker.GenerateQuadTreeConnectedNode();
            cellMaker.MakeConnected();

            cellMaker.CollectOuterQuadRect();
            cellMaker.CollectInnerQuadRect();

            SceneView.RepaintAll();
            Debug.Log("Generate");
        }

        if (GUILayout.Button("TestIsIntersect"))
        {
            cellMaker.TestIsIntersect();
        }
    }
}

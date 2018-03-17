using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxDirection { Default,FromBlender}

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderMetaInfo : MonoBehaviour {

    public BoxCollider boxCollider;
    public BoxDirection boxDirection = BoxDirection.Default;

}

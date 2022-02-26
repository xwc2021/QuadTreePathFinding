using UnityEngine;
public class ShootingRay : MonoBehaviour
{

    public Transform target;
    Camera c;
    // Use this for initialization
    void Start()
    {
        c = Camera.main;
    }

    Ray ray;
    float maxDistance = 500;
    RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = c.ScreenPointToRay(Input.mousePosition);

            var layerMask = 1 << 8;
            Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, layerMask);
            if (target != null)
                target.position = hit.point;
        }

        Debug.DrawLine(hit.point, hit.point + hit.normal * 2, Color.blue);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100);
    }
}
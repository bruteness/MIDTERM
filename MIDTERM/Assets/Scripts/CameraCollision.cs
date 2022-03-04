using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;
    public float smooth = 10.0f;
    public Vector3 dollyDirAdjusted;
    public float distance;
    
    private Vector3 dollyDir;
    private float dollyDirStartY;
    
    // Start is called before the first frame update
    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        dollyDirStartY = dollyDir.y;
        distance = transform.localPosition.magnitude;    
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if(Physics.Linecast (transform.parent.position, desiredCameraPos, out hit))
        {
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, maxDistance);
            if(dollyDir.y < 1)
            {
                dollyDir = new Vector3(dollyDir.x, dollyDir.y + .4f, dollyDir.z);
            }
        }
        else
        {
            distance = maxDistance;
            if(dollyDir.y > dollyDirStartY)
            dollyDir = new Vector3(dollyDir.x, dollyDir.y - .4f, dollyDir.z);
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}

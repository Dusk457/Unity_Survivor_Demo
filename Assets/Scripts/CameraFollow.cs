using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;

    [Header("偏移位置")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("移动速度")]
    public float smoothSpeed = 5f;

    private void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime); 
    }
}

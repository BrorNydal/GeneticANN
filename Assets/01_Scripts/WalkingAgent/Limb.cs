using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour
{
    [SerializeField] float minLimit;
    [SerializeField] float maxLimit;
    [SerializeField] bool relative = false;

    private void Awake()
    {
        if (relative)
        {
            minLimit = transform.rotation.eulerAngles.z - minLimit;
            maxLimit = transform.rotation.eulerAngles.z + maxLimit;
        }
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + min);
        return Mathf.Min(angle, max);
    }

    public void Rotate(float delta)
    {
        transform.rotation = 
            Quaternion.Euler(
                new Vector3
                (
                    transform.rotation.eulerAngles.x, 
                    transform.rotation.eulerAngles.y, 
                    ClampAngle(transform.rotation.eulerAngles.z + delta, minLimit, maxLimit)
                )
            );
    }
}

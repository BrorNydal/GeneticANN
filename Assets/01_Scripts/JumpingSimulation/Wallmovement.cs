using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallmovement : MonoBehaviour
{
    [SerializeField] float autoKillX = -12f;
    [SerializeField] float speed = -1f;

    public delegate void WallKill();
    public static event WallKill OnKill;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < autoKillX)
            Destroy(gameObject);

        transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
    }
}

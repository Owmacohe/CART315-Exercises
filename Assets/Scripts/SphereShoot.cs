using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereShoot : MonoBehaviour
{
    private void FixedUpdate()
    {
        RaycastHit hitObject;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitObject, Mathf.Infinity))
        {
            if (Random.Range(0, 10) <= 1)
            {
                GameObject newSphere = Instantiate(Resources.Load<GameObject>("Sphere"), transform.localPosition + (Vector3.up * 10), transform.localRotation);
            }
        }
    }
}

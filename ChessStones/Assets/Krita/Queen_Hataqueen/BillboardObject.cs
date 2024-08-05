using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BillboardObject : MonoBehaviour
{
    private Vector3 eulers;
    void Update()
    {
        //eulers = transform.eulerAngles;
        //eulers.y = Camera.main.transform.eulerAngles.y;
        //transform.eulerAngles = eulers;
        transform.forward = Camera.main.transform.forward;
    }
}

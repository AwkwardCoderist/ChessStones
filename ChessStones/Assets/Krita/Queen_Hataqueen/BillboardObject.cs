using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BillboardObject : MonoBehaviour
{

    public Vector3 Clamp;

    private Vector3 _eulers;

    void Update()
    {
        //eulers = transform.eulerAngles;
        //eulers.y = Camera.main.transform.eulerAngles.y;
        //transform.eulerAngles = eulers;
        transform.forward = Camera.main.transform.forward;

        _eulers = transform.localEulerAngles;
        _eulers.x = Mathf.Clamp(_eulers.x, -Clamp.x, Clamp.x);
        _eulers.y = Mathf.Clamp(_eulers.y, -Clamp.y, Clamp.y);
        _eulers.z = Mathf.Clamp(_eulers.z, -Clamp.z, Clamp.z);
        transform.localEulerAngles = _eulers;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugTest : MonoBehaviour
{
    public float reloadScenePress = 5;
    private float elapsedReload = 0;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            elapsedReload += Time.deltaTime;
            if(elapsedReload >= reloadScenePress ) 
            {
                SceneManager.LoadScene(0);
            }
        }
        else if(elapsedReload > 0)
        {
            elapsedReload = 0;
        }

    }
}

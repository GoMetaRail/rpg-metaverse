using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GMRGate : MonoBehaviour
{
    private float startupTime;
    private bool ignoreTriggerEnter = false;

    void Start() {
        startupTime = Time.realtimeSinceStartup;
        Debug.Log("Start time: " + startupTime.ToString());
    }

    void OnTriggerEnter (Collider other) {
        // Ignore if the player entered at the gate
        float diff = (Time.realtimeSinceStartup - startupTime);
        Debug.Log(diff.ToString());
        if(ignoreTriggerEnter || (Time.realtimeSinceStartup - startupTime) <= 7) {
            Debug.Log("Ignored");
            ignoreTriggerEnter = true;
        } else {
            SceneManager.LoadScene("GMRTransitionScene");
        }
    }

    void OnTriggerStay (Collider other) {
        // Debug.Log("Stay gate");
    }

    void OnTriggerExit (Collider other) {
        ignoreTriggerEnter = false;
    }
}

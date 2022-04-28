using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMRGate : MonoBehaviour
{
    void OnTriggerEnter (Collider other) {
        Debug.Log("Entered gate");
        // todo: Initiate Transfer
        // todo: Get URL of destination
        // todo: Launch destination
        Debug.Log("Launching destination");
        // Application.ExternalEval("window.location.href = \"https://scifi.prototype.gometarail.io/\"");
    }

    void OnTriggerStay (Collider other) {
    }

    void OnTriggerExit (Collider other) {
    }
}

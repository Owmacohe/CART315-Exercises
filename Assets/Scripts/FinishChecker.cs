using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishChecker : MonoBehaviour
{
    private bool hasLanded;

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;

        if (!hasLanded && otherObject.tag.Equals("Player")) {
            hasLanded = true;

            otherObject.GetComponentInChildren<TMP_Text>().text = "Congratulations!";
            Invoke("resetPlayer", 1);
        }
    }

    void resetPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").transform.localPosition = Vector3.up;
        FindObjectOfType<TMP_Text>().text = "";
    }
}

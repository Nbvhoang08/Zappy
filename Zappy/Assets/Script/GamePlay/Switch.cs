using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private int doorID;
    public GameObject turnOnObject;

     void OnTriggerStay2D(Collider2D other)
    {
        // Nếu chưa có đối tượng nào được gán và đối tượng khác có Collider2D với isTrigger = true
        if (turnOnObject == null && other.isTrigger)
        {
            turnOnObject = other.gameObject;
            Subject.NotifyObservers("Open",doorID);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Nếu đối tượng rời đi là đối tượng đang được gán
        if (other.gameObject == turnOnObject)
        {
            turnOnObject = null;
            Subject.NotifyObservers("Close",doorID);
        }
    }
}

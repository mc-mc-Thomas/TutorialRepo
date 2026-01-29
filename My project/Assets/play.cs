using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class play : MonoBehaviour
{
    public GameObject gameobject;

    public void playBTN()
    {
        gameobject.SetActive(false);
    }
}

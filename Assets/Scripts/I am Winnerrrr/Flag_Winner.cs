using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag_Winner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Trigger Win Function
        Game_Manager.instance.Win();
    }
}

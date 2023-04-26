using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    //[SerializeField] int ammoAmount = 10; old (when there is on ammo for all weapons)

    [SerializeField] AmmoSlot[] ammoSlots;

    [System.Serializable] //show the content below this class or the context
    private class AmmoSlot{
        public AmmoType ammoType; //Rocket, Bullet , Shells
        public int ammoAmount;
}

    public int GetCurrentAmmo(AmmoType ammoType)
    {
        return GetAmmoSlot(ammoType).ammoAmount;
    }

    public void ReduceCurrentAmmo(AmmoType ammoType)
    {
        GetAmmoSlot(ammoType).ammoAmount--;
    }

    public void IncreaseCurrentAmmo(AmmoType ammoType , int ammoAmount)
    {
        GetAmmoSlot(ammoType).ammoAmount += ammoAmount; //for increase the ammo

    }

    private AmmoSlot GetAmmoSlot(AmmoType ammoType)
    {
        foreach(AmmoSlot slot in ammoSlots)
        {
            if(slot.ammoType == ammoType)
            {
                return slot; //yes this is the particular slots where the player in (use it)
            }
        }
        return null; //because I enter an input in this method we heya ht3ml output bs fy function tanyah 2et2al
    }
}

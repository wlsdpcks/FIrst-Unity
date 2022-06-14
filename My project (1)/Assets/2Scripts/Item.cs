using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Weapon, Grenade, Ammo };
    public Type type;
    public int value;

    void Update()
    {
        if(type == Type.Weapon)
            transform.Rotate(Vector3.up * 10 * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Powerup")]
public class Powerup : ScriptableObject
{
    public new string name;
    public float jump;
    public float speed;
    public bool immunity;
    public int health;

    public void Print()
    {
        Debug.Log("Name of powerup" + name);
    }

}

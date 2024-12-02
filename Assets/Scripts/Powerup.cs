using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Powerup")]
public class Powerup : ScriptableObject
{
    public new string name;
    public float jumpForce;
    public float speed;
    public bool immunity;
    public int health;
    public Color color;

    public void Print ()
    {
        Debug.Log("powerup " + name + " created");
    }

    public virtual void ApplyPowerupEffect(PlayerController player)
    {
        player.jumpForce = jumpForce;
        player.moveSpeed = speed;
        player.immunity = immunity;
        player.addHealth(health);
        if(health==0)
        {
            player.ApplyColorTint(color);
        }
    }
    public virtual void RemovePowerupEffect(PlayerController player)
    {
        player.jumpForce = 10; 
        player.moveSpeed = 5; 
        player.immunity = false;
        player.ApplyColorTint(player.originalColor);
    }


}

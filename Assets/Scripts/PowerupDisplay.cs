using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupDisplay : MonoBehaviour
{
    
    public Powerup powerup;

    // Start is called before the first frame update
    void Start()
    {
        powerup.Print();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] 
    public TMP_Text velocityTxt;
    // Start is called before the first frame update
    public void UpdateVelocityTxt(string v)
    {
        velocityTxt.text = "Velocity: " + v;
    }
}

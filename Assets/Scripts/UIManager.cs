using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    [SerializeField] 
    public TMP_Text infoReport;

    //private PlayerMain playerMain;
    // Start is called before the first frame update
    void Start()
    {
        //playerMain = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
    }
    public void InfoReport(string v, string state)
    {
        infoReport.text = "Velocity: " + v + "\n" +
                        "State: " + state + "\n";
    }

    
}

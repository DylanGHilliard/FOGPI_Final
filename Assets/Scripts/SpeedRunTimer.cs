using TMPro;
using UnityEngine;

public class SpeedRunTimer : MonoBehaviour
{
    public TMP_Text timerText;
    private float time;
    public WinScript winScript;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!winScript.hasWon){
            time += Time.deltaTime;
            string formatedTime = System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss\:ff");
            timerText.text = "Time: "+ formatedTime;
        }
    }
}

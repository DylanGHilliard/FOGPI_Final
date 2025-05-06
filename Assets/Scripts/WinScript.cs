using TMPro;
using UnityEngine;

public class WinScript : MonoBehaviour
{
    public TMP_Text winText;

    public bool hasWon;
    private bool isOn = true;
    private float time;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasWon){
            time += Time.unscaledDeltaTime;
            
            if (time > 0.2f)
            {
                isOn = !isOn;
                winText.gameObject.SetActive(isOn);
                time = 0;

            }
        }
    }

    public void WinState()
    {
        hasWon = true;
        winText.gameObject.SetActive(true);
        winText.text = "You Win!";
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

using UnityEngine;
using UnityEngine.Events;

public class Wallet : MonoBehaviour
{
   public int score = 0; 
   public int winScore = 10; // Score needed to win
   public UnityEvent unityEvent;
    void Start()
    {
        
    }

    void Update()
    {
        if (score > winScore){
            unityEvent.Invoke();
        }
    }
    public void AddScore(int _amount)
    {
        score += _amount;
    }
}

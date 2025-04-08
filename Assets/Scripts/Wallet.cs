using UnityEngine;

public class Wallet : MonoBehaviour
{
   public int score = 0; 
    void Start()
    {
        
    }
    public void AddScore(int _amount)
    {
        score += _amount;
    }
}

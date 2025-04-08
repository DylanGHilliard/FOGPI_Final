using UnityEngine;

public class Collectable : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 0.5f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add score to the player
            Wallet player = other.GetComponent<Wallet>();
            if (player != null)
            {
                player.AddScore(1);
            }
            
            // Destroy the collectable
            Destroy(gameObject);
        }
    }
}

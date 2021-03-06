using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : MonoBehaviour
{
    // Method: Invoked when another collider comes into contact
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If: What came on contact was not the player..
        if(collision.gameObject.tag != "Player")
        {
            return;
        } 

        // Retrieve the current value of the players current and max health
        float currHealth = FindObjectOfType<PlayerHealth>().CurrentHealth();
        float maxHealth = FindObjectOfType<PlayerHealth>().MaxHealth();
        
        // If: The players current health is at max capacity...
        if(currHealth == maxHealth)
        {
            return;
        }

        // Increase the players health and update the UI    
        FindObjectOfType<PlayerHealth>().GetHealth();
        FindObjectOfType<GameController>().uiUpdate();

        SoundManager.Instance.PlayClip("Enemy Swing");
        
        // Destroy the game object
        Destroy(gameObject);
    }
}

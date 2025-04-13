using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
   
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Trigger");
           
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                player.AddStamina(20f);  // Exemple : ajoute 20 points de stamina
            }

            animator.SetBool("Collectible", true);
            Destroy(gameObject, 1f);
        }
    }
   
}

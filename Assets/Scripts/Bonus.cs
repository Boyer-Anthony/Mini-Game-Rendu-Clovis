using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
   
    public Animator animator;
    public GameObject bonusPrefab;

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

            StartCoroutine(RespawnBonus());
            //Destroy(gameObject, 1f);
        }
    }

    private IEnumerator RespawnBonus()
    {
        animator.SetBool("Collectible", true);
        
        yield return new WaitForSeconds(1f);
        bonusPrefab.SetActive(false);
        yield return new WaitForSeconds(2f);
        bonusPrefab.SetActive(true);
        animator.SetBool("Show", true) ;
    }
   
}

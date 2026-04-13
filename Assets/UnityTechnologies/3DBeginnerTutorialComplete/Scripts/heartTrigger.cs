using UnityEngine;

public class heartTrigger : MonoBehaviour
{
    public AudioSource heart; //Audio player object, on John Lemon
    private short num = 0; // Track # ghosts in circle (stops a ghost from ending noise if another ghost is stil inside range)
    void OnTriggerEnter(Collider other)
    {
        // Detect object entering trigger collider
        // Check if it is a ghost
        if (other.gameObject.CompareTag("Ghost"))
        {
            num += 1; // increment tally of ghosts in trigger
            if (!heart.isPlaying)
            {
                heart.Play(); // if not already playing then play (avoid restarting on accident)
            } 
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        // Detect object leaving trigger collider
        // check if it is a ghost
        if (other.gameObject.CompareTag("Ghost"))
        {
            // decrement counter
            num -= 1;
            if(num == 0)
            {
                heart.Stop(); // if the last ghost left turn off sound
            }
        }
    }
}

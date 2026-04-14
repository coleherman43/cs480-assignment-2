using UnityEngine;

public class heartTrigger : MonoBehaviour
{
    public AudioSource heart; //Audio player object, on John Lemon
    public PlayerMovement playerMovement; // Reference to grab closestDist from Kobe's logic
    public float minDistance = 3f;  // Full volume at this distance or closer
    public float maxDistance = 7.5f; // Silent at this distance or farther — matches Kobe's maxDist
    public float smoothSpeed = 2f; // Adjust in Inspector to taste

    private short num = 0; // Track # ghosts in circle (stops a ghost from ending noise if another ghost is still inside range)
    private float targetVolume = 0f; // Target volume to lerp toward

    void Start()
    {
        // Find PlayerMovement on the parent John Lemon object
        playerMovement = GetComponentInParent<PlayerMovement>();
    }
    
    void Update()
    {
        // Only check distance if a ghost is in the trigger zone
        if (num > 0)
        {
            // Reuse Kobe's already-computed nearest ghost distance from PlayerMovement
            targetVolume = Mathf.InverseLerp(maxDistance, minDistance, playerMovement.closestDist);
        }
        else
        {
            targetVolume = 0f;
        }

        // Smoothly lerp volume toward target each frame
        heart.volume = Mathf.Lerp(heart.volume, targetVolume, Time.deltaTime * smoothSpeed);

        // Start playing when fading in, stop when fully faded out
        if (targetVolume > 0 && !heart.isPlaying)
        {
            heart.volume = 0f;
            heart.Play();
        }
        else if (targetVolume == 0 && heart.volume < 0.01f && heart.isPlaying)
        {
            heart.Stop();
            heart.volume = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Detect object entering trigger collider
        // Check if it is a ghost
        if (other.gameObject.CompareTag("Ghost"))
        {
            num += 1; // increment tally of ghosts in trigger
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
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;

    // Add a variable for the Ghost Indicator arrow
    public RectTransform ghostIndicator;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    // Define an array of GameObjects to hold all of the ghosts
    GameObject[] ghosts;

    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();

        // Fill our ghosts array with all the current ghosts in the game
        // Note: Each of the 4 ghosts have a "Ghost" tag
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");
    }

    void FixedUpdate ()
    {
        float horizontal = Input.GetAxis ("Horizontal");
        float vertical = Input.GetAxis ("Vertical");
        
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize ();

        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);
        
        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop ();
        }

        Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation (desiredForward);


        // Initialize variables to hold the closest Ghost distance and the index of that ghost
        float closestDist = float.PositiveInfinity;
        int ghostIdx = 0;
        Vector3 toGhost = Vector3.zero;

        // Loop through all the ghosts
        for (int i = 0 ; i < ghosts.Length ; i++)
        {
            GameObject ghost = ghosts[i];
            // Compute the vector pointing from the player to the current ghost
            toGhost = (ghost.transform.position - transform.position);
            // Compute the distance from the player to the ghost
            float distanceToGhost = toGhost.magnitude;
            if (distanceToGhost < closestDist)
            {
                closestDist = distanceToGhost;
                ghostIdx = i;
            }
        }

        // Get the main camera transform object
        Transform cam = Camera.main.transform;
        GameObject closestGhost = ghosts[ghostIdx];
        // Compute the normalized vector from the player to the nearest ghost, and the dot products between that vector and the main camera
        toGhost = (closestGhost.transform.position - transform.position).normalized;
        float forwardDot = Vector3.Dot(cam.forward, toGhost);
        float rightDot = Vector3.Dot(cam.right, toGhost);
        float angle = Mathf.Atan2(rightDot, forwardDot) * Mathf.Rad2Deg;

        // Rotate the Ghost Indicator Arrow
        ghostIndicator.rotation = Quaternion.Euler(0, 0, -angle + 90f);

        // Modify the Opacity of the Ghost indicator arrow based on how close the ghost is
        float maxDist = 7.5f;
        float t = Mathf.Clamp01(1f - (closestDist / maxDist));
        float alpha = Mathf.Lerp(0.2f, 1f, t);

        UnityEngine.UI.Image img = ghostIndicator.GetComponent<UnityEngine.UI.Image>();
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    void OnAnimatorMove ()
    {
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation (m_Rotation);
    }
}
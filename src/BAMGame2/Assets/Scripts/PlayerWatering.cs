using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerWatering : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public PlayerMovement playerMovement;
    public float wateringDuration = 0.8f;

    private Rigidbody2D rb;
    private bool isWatering = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isWatering && Keyboard.current.fKey.wasPressedThisFrame)
        {
            StartCoroutine(HandleWatering());
        }
    }

    private IEnumerator HandleWatering()
    {
        isWatering = true;

        // 🛑 Stop movement immediately
        rb.linearVelocity = Vector2.zero;
        playerMovement.enabled = false;

        // 🎬 Play watering animation
        animator.SetBool("isWatering", true);

        // 🔁 Ensure facing direction is preserved
        Vector2 facingDir = playerMovement.LastMoveDir;
        animator.SetFloat("LastInputX", facingDir.x);
        animator.SetFloat("LastInputY", facingDir.y);

        // 💧 Water crops nearby
        if (FarmManager.Instance != null)
            FarmManager.Instance.WaterNearbyCrops(transform.position, FarmManager.Instance.wateringRadius);

        // Wait for the animation to finish
        yield return new WaitForSeconds(wateringDuration);

        // ✅ Resume movement
        animator.SetBool("isWatering", false);
        playerMovement.enabled = true;
        isWatering = false;
    }
}
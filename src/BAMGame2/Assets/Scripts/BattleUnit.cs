using UnityEngine;
using TMPro;
using System.Collections;

public class BattleUnit : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP;
    public int currentHP;
    public int damage;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public TMP_Text hpText;
    public TMP_Text dmgText;

    [Header("Animation Settings")]
    public float attackTiltAngle = 45f;
    public float attackTiltSpeed = 0.15f;
    public float hitFlashDuration = 0.15f;

    // Determines whether this unit tilts left or right during attack
    public bool facesLeft = false;

    public bool IsDead => currentHP <= 0;

    // ----------------------------------------------------
    // INITIALIZE UNIT
    // ----------------------------------------------------
    public void Initialize(int hp, int dmg)
    {
        maxHP = hp;
        currentHP = hp;
        damage = dmg;

        UpdateUI();
    }

    // ----------------------------------------------------
    // UI UPDATE
    // ----------------------------------------------------
    private void UpdateUI()
    {
        if (hpText != null)
            hpText.text = $"HP: {currentHP}";

        if (dmgText != null)
            dmgText.text = $"DMG: {damage}";
    }

    // ----------------------------------------------------
    // TAKE DAMAGE
    // ----------------------------------------------------
    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        StartCoroutine(HitFlash());
        UpdateUI();
    }

    // ----------------------------------------------------
    // ATTACK ANIMATION (mirrored based on facesLeft)
    // ----------------------------------------------------
    public IEnumerator PlayAttackAnimation()
    {
        Vector3 originalRot = transform.localEulerAngles;

        float angle = attackTiltAngle;

        // Mirror tilt direction if facing left
        if (facesLeft)
            angle = -attackTiltAngle;

        Vector3 tilted = new Vector3(0, 0, angle);

        // Tilt
        transform.localEulerAngles = tilted;
        yield return new WaitForSeconds(attackTiltSpeed);

        // Return
        transform.localEulerAngles = originalRot;
        yield return new WaitForSeconds(attackTiltSpeed);
    }

    // ----------------------------------------------------
    // FLASH WHEN HIT
    // ----------------------------------------------------
    private IEnumerator HitFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(hitFlashDuration);
            spriteRenderer.color = Color.white;
        }
    }
}
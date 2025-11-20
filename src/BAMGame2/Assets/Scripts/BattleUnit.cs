using System.Collections;
using TMPro;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP;
    public int currentHP;
    public int damage;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public TMP_Text hpText;

    [Header("Animation Settings")]
    public float attackTiltAngle = 45f;
    public float attackTiltSpeed = 0.15f;
    public float hitFlashDuration = 0.15f;

    public bool IsDead => currentHP <= 0;

    private Color defaultColor;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        defaultColor = spriteRenderer.color;
    }

    private void Start()
    {
        UpdateHPText();
    }

    
    // PUBLIC — Init stats
    
    public void Initialize(int hp, int dmg)
    {
        maxHP = hp;
        currentHP = hp;
        damage = dmg;
        UpdateHPText();
    }

    
    // PUBLIC — Attack animation (tilt forward)
    
    public IEnumerator PlayAttackAnimation()
    {
        // Tilt forward
        yield return TiltTo(attackTiltAngle);

        // Tilt back
        yield return TiltTo(0f);
    }

    private IEnumerator TiltTo(float angle)
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);

        float t = 0f;
        while (t < attackTiltSpeed)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t / attackTiltSpeed);
            yield return null;
        }
        transform.rotation = targetRot;
    }

    
    // PUBLIC — Take Damage
    
    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        StartCoroutine(HitFlash());

        UpdateHPText();

        if (currentHP == 0)
        {
            Die();
        }
    }

    
    // PUBLIC — Flash Red on Hit
    private IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = defaultColor;
    }

    
    // PUBLIC — Death animation
    
    private void Die()
    {
        StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation()
    {
        // Gray out
        spriteRenderer.color = Color.gray;

        // Flip upside down
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(0, 0, 180f);

        float t = 0;
        while (t < 0.25f)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, endRot, t / 0.25f);
            yield return null;
        }

        transform.rotation = endRot; 
    }

    // UPDATE HP UI
    
    public void UpdateHPText()
    {
        if (hpText != null)
            hpText.text = currentHP.ToString();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("Battle Prefabs")]
    public GameObject animalBattlePrefab;
    public GameObject skeletonBattlePrefab;

    [Header("Spawn Points")]
    public Transform[] animalSpawnPoints;
    public Transform[] skeletonSpawnPoints;

    [Header("UI Popup")]
    public BattlePopupUI popupUI;

    private List<BattleUnit> animalUnits = new List<BattleUnit>();
    private List<BattleUnit> skeletonUnits = new List<BattleUnit>();

    private int nightIndex = 1;
    private bool battleRunning = false;

    private void Start()
    {
        LoadNightIndex();
        SpawnAllUnits();
        StartCoroutine(BattleLoop());
    }

    private void LoadNightIndex()
    {
        nightIndex = Mathf.Clamp(PlayerPrefs.GetInt("Night", 1), 1, 3);
        Debug.Log("[BATTLE] Loaded night = " + nightIndex);
    }

    private void SaveNightIndex(int value)
    {
        PlayerPrefs.SetInt("Night", value);
    }

    // ----------------------------------------------------------
    // SPAWNING
    // ----------------------------------------------------------
    private void SpawnAllUnits()
    {
        animalUnits.Clear();
        skeletonUnits.Clear();

        SpawnAnimals();
        SpawnSkeletonsForNight(nightIndex);

        if (animalUnits.Count == 0)
        {
            Debug.LogWarning("[BATTLE] No animals available – automatic loss.");
            StartCoroutine(HandleLose());
            return;
        }

        battleRunning = true;
    }

    private void SpawnAnimals()
    {
        var owned = PlayerAnimalInventory.Instance.ownedAnimals;

        for (int i = 0; i < owned.Count && i < animalSpawnPoints.Length; i++)
        {
            var def = owned[i];
            if (def == null) continue;

            GameObject obj = Instantiate(animalBattlePrefab,
                                         animalSpawnPoints[i].position,
                                         Quaternion.identity);

            BattleUnit unit = obj.GetComponent<BattleUnit>();
            if (unit == null) continue;

            // Assign stats
            unit.Initialize(def.hp, def.damage);

            // Assign sprite (animals face left)
            if (unit.spriteRenderer != null)
            {
                unit.spriteRenderer.sprite = def.battleSprite;
                unit.spriteRenderer.flipX = true;
            }

            unit.facesLeft = true;
            animalUnits.Add(unit);
        }
    }

    private void SpawnSkeletonsForNight(int night)
    {
        List<SkeletonDefinition> defs = SkeletonConfig.Instance.GetSkeletonsForNight(night);

        for (int i = 0; i < defs.Count && i < skeletonSpawnPoints.Length; i++)
        {
            var def = defs[i];
            if (def == null) continue;

            GameObject obj = Instantiate(skeletonBattlePrefab,
                                         skeletonSpawnPoints[i].position,
                                         Quaternion.identity);

            BattleUnit unit = obj.GetComponent<BattleUnit>();
            if (unit == null) continue;

            unit.Initialize(def.hp, def.damage);

            // Skeletons face right → no flipping needed unless your sprite faces left by default.
            if (unit.spriteRenderer != null)
                unit.spriteRenderer.flipX = false;

            unit.facesLeft = false;

            skeletonUnits.Add(unit);
        }
    }

    // ----------------------------------------------------------
    // BATTLE LOOP
    // ----------------------------------------------------------
    private IEnumerator BattleLoop()
    {
        // 1. INTRO POPUP
        if (popupUI != null)
        {
            popupUI.Show($"Night {nightIndex} - Survive!");
            yield return new WaitForSeconds(2f);
            popupUI.Hide();
        }

        // Allow popup to fade out cleanly
        yield return new WaitForSeconds(0.5f);

        if (!battleRunning) yield break;

        // 2. BATTLE ROUNDS
        while (battleRunning)
        {
            yield return StartCoroutine(AnimalsAttack());

            if (AllDead(skeletonUnits))
            {
                battleRunning = false;
                yield return StartCoroutine(ShowWinPopup());
                yield return HandleWin();
                yield break;
            }

            yield return StartCoroutine(SkeletonsAttack());

            if (AllDead(animalUnits))
            {
                battleRunning = false;
                yield return StartCoroutine(ShowLosePopup());
                yield return HandleLose();
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator ShowWinPopup()
    {
        if (popupUI != null)
        {
            popupUI.Show("Victory! You made it through the night");
            yield return new WaitForSeconds(2f);
            popupUI.Hide();
        }
    }

    private IEnumerator ShowLosePopup()
    {
        if (popupUI != null)
        {
            popupUI.Show("Defeat... Retreat!");
            yield return new WaitForSeconds(2f);
            popupUI.Hide();
        }
    }

    // ----------------------------------------------------------
    // ATTACK PHASES
    // ----------------------------------------------------------
    private IEnumerator AnimalsAttack()
    {
        foreach (var unit in animalUnits)
        {
            if (unit == null || unit.IsDead) continue;

            var target = PickRandomAlive(skeletonUnits);
            if (target == null) yield break;

            yield return unit.PlayAttackAnimation();
            target.TakeDamage(unit.damage);

            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator SkeletonsAttack()
    {
        foreach (var unit in skeletonUnits)
        {
            if (unit == null || unit.IsDead) continue;

            var target = PickRandomAlive(animalUnits);
            if (target == null) yield break;

            yield return unit.PlayAttackAnimation();
            target.TakeDamage(unit.damage);

            yield return new WaitForSeconds(0.25f);
        }
    }

    // ----------------------------------------------------------
    // HELPERS
    // ----------------------------------------------------------
    private BattleUnit PickRandomAlive(List<BattleUnit> list)
    {
        var alive = list.FindAll(u => u != null && !u.IsDead);
        if (alive.Count == 0) return null;

        return alive[Random.Range(0, alive.Count)];
    }

    private bool AllDead(List<BattleUnit> list)
    {
        foreach (var u in list)
        {
            // Must check both null AND IsDead
            if (u != null && !u.IsDead)
                return false;
        }

        return true;
    }

    // ----------------------------------------------------------
    // WIN/LOSE HANDLING
    // ----------------------------------------------------------
    private IEnumerator HandleWin()
    {
       
        if (popupUI != null)
        {
            popupUI.Show("Victory! You made it through the night");
            yield return new WaitForSeconds(2f);
            popupUI.Hide();
        }

        nightIndex++;
        SaveNightIndex(nightIndex);
        
        if (nightIndex > 3)
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("End");
            yield break;
        }
        
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game");
    }

    private IEnumerator HandleLose()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game");
    }
}
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
        // ❗ No clamp here — we WANT nightIndex > 3 after winning night 3
        nightIndex = PlayerPrefs.GetInt("Night", 1);
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

        // Clamp ONLY for skeleton spawning
        int spawnNight = Mathf.Clamp(nightIndex, 1, 3);
        SpawnSkeletonsForNight(spawnNight);

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

            unit.Initialize(def.hp, def.damage);

            // Animals always face left
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

            // Skeletons face right
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
        // Intro popup
        if (popupUI != null)
        {
            popupUI.Show($"Night {Mathf.Clamp(nightIndex, 1, 3)} - Survive!");
            yield return new WaitForSeconds(2f);
            popupUI.Hide();
        }

        yield return new WaitForSeconds(0.5f);
        if (!battleRunning) yield break;

        // Combat
        while (battleRunning)
        {
            yield return StartCoroutine(AnimalsAttack());

            if (AllDead(skeletonUnits))
            {
                battleRunning = false;
                yield return ShowWinPopup();
                yield return HandleWin();
                yield break;
            }

            yield return StartCoroutine(SkeletonsAttack());

            if (AllDead(animalUnits))
            {
                battleRunning = false;
                yield return ShowLosePopup();
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
            if (u != null && !u.IsDead)
                return false;

        return true;
    }

    // ----------------------------------------------------------
    // WIN/LOSE HANDLING
    // ----------------------------------------------------------
    private IEnumerator HandleWin()
    {
        if (nightIndex >= 3)
        {
            // This WAS the final night!
            SceneManager.LoadScene("End");
            yield break;
        }

        // NOT final night → move forward normally
        nightIndex++;
        SaveNightIndex(nightIndex);
        SceneManager.LoadScene("Game");
    }

    private IEnumerator HandleLose()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game");
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    public List<Spell> allSpells;
    public Transform spellUIParent;
    public GameObject spellUIPrefab;
    public int maxSpellsInUI = 4;

    public LeftPerson leftPerson;
    public RightPerson rightPerson;
    public Boss boss;

    private List<Spell> currentSpells = new List<Spell>();
    private Dictionary<Spell, GameObject> spellToUIGameObject = new Dictionary<Spell, GameObject>();

    [SerializeField] private AudioSource hitBossSound;
    [SerializeField] private AudioSource shieldSound;
    [SerializeField] private AudioSource healSound;

    private void Start()
    {
        for (int i = 0; i < maxSpellsInUI; i++)
        {
            AddRandomSpellToUI();
        }
    }

    private void AddRandomSpellToUI()
    {
        List<Spell> availableSpells = new List<Spell>(allSpells);
        availableSpells.RemoveAll(s => currentSpells.Contains(s));

        if (availableSpells.Count > 0)
        {
            Spell newSpell = availableSpells[Random.Range(0, availableSpells.Count)];
            currentSpells.Add(newSpell);

            GameObject spellUIObj = Instantiate(spellUIPrefab, spellUIParent);
            Image spellUIImage = spellUIObj.GetComponent<Image>();
            if (spellUIImage != null)
            {
                spellUIImage.sprite = newSpell.spellSprite;
                spellUIImage.preserveAspect = true;
            }

            spellToUIGameObject[newSpell] = spellUIObj;
        }
    }

    public bool CheckPattern(int[] pattern)
    {
        foreach (Spell spell in currentSpells)
        {
            if (IsPatternMatch(pattern, spell.pattern))
            {
                ApplySpellEffect(spell);
                RemoveSpell(spell);
                return true;
            }
        }
        return false;
    }

    private bool IsPatternMatch(int[] inputPattern, int[] spellPattern)
    {
        if (inputPattern.Length != spellPattern.Length)
            return false;

        for (int i = 0; i < inputPattern.Length; i++)
        {
            if (inputPattern[i] != spellPattern[i])
                return false;
        }
        return true;
    }

    private void RemoveSpell(Spell spell)
    {
        currentSpells.Remove(spell);

        if (spellToUIGameObject.ContainsKey(spell))
        {
            Destroy(spellToUIGameObject[spell]);
            spellToUIGameObject.Remove(spell);
        }

        AddRandomSpellToUI();
    }

    private void ApplySpellEffect(Spell spell)
    {
        switch (spell.spellType)
        {
            case SpellType.Attack:
                if (hitBossSound != null)
                    hitBossSound.Play();
                if (boss != null)
                {
                    boss.TakeDamage(spell.damage);
                }
                break;
            case SpellType.Recover:
                if (healSound != null)
                    healSound.Play();
                if (leftPerson != null)
                {
                    leftPerson.RecoverHealth(8);  
                    leftPerson.StartCoroutine(leftPerson.ChangeColorTemporary(Color.yellow, 2f));
                }
                if (rightPerson != null)
                {
                    rightPerson.StartCoroutine(rightPerson.ChangeColorTemporary(Color.yellow, 2f));
                }
                break;
            case SpellType.Shield:
                if (shieldSound != null)
                    shieldSound.Play();
                if (leftPerson != null)
                {
                    leftPerson.StartCoroutine(leftPerson.ActivateShield(2f));
                }
                if (rightPerson != null)
                {
                    rightPerson.StartCoroutine(rightPerson.ActivateShield(2f));
                }
                break;
        }
    }

    public void OnSpellFailed()
    {
    }
}

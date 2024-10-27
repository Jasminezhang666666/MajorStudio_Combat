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
    public Boss boss;                      
    public int manaCostPerSpell = 5;       

    private List<Spell> currentSpells = new List<Spell>(); 
    private Dictionary<Spell, GameObject> spellToUIGameObject = new Dictionary<Spell, GameObject>();

    private void Start()
    {
        Debug.Log("SpellManager Start: Initializing spells.");
        // Initialize the current spells and UI
        for (int i = 0; i < maxSpellsInUI; i++)
        {
            AddRandomSpellToUI();
        }
    }

    private void AddRandomSpellToUI()
    {
        Debug.Log("Attempting to add a random spell to the UI.");
        // Choose a random spell not already displayed
        List<Spell> availableSpells = new List<Spell>(allSpells);
        availableSpells.RemoveAll(s => currentSpells.Contains(s));

        if (availableSpells.Count > 0)
        {
            Spell newSpell = availableSpells[Random.Range(0, availableSpells.Count)];
            currentSpells.Add(newSpell);

            // Instantiate UI image from prefab
            GameObject spellUIObj = Instantiate(spellUIPrefab, spellUIParent);
            Image spellUIImage = spellUIObj.GetComponent<Image>();
            if (spellUIImage != null)
            {
                spellUIImage.sprite = newSpell.spellSprite;
                spellUIImage.preserveAspect = true;
                Debug.Log("Assigned sprite to spell UI image: " + newSpell.spellName);
            }
            else
            {
                Debug.LogError("Spell UI prefab does not have an Image component.");
            }

            // Map the spell to its UI GameObject
            spellToUIGameObject[newSpell] = spellUIObj;
        }
        else
        {
            Debug.Log("No more spells to add to UI.");
        }
    }

    public void DeductMana()
    {
        // Decrease left player's health/mana when a spell is attempted
        if (leftPerson != null)
        {
            leftPerson.TakeDamage(manaCostPerSpell);
            Debug.Log("Mana deducted by " + manaCostPerSpell + ". Current health/mana: " + leftPerson.health);
        }
    }

    public bool CheckPattern(int[] pattern)
    {
        foreach (Spell spell in currentSpells)
        {
            if (IsPatternMatch(pattern, spell.pattern))
            {
                // Pattern matches this spell
                Debug.Log("Spell matched: " + spell.spellName);

                // Apply spell effect
                ApplySpellEffect(spell);

                // Remove spell from UI and current spells
                RemoveSpell(spell);

                return true;
            }
        }
        // No match found
        Debug.Log("No matching spell found.");
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

        // Destroy the UI GameObject
        if (spellToUIGameObject.ContainsKey(spell))
        {
            Destroy(spellToUIGameObject[spell]);
            spellToUIGameObject.Remove(spell);
        }

        // Add a new spell to the UI to maintain the maximum number
        AddRandomSpellToUI();
    }

    private void ApplySpellEffect(Spell spell)
    {
        // Apply the spell's effect on the boss
        if (boss != null)
        {
            boss.TakeDamage(spell.damage);
        }
    }

    public void OnSpellFailed()
    {
        Debug.Log("Spell attempt failed.");
        // You can add additional feedback here, such as playing a sound or showing a message
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellType { Attack, Recover, Shield }

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
public class Spell : ScriptableObject
{
    public string spellName;
    public Sprite spellSprite;
    public int[] pattern; 
    public int damage = 10;
    public float cooldown = 5f;
    public SpellType spellType; 
}

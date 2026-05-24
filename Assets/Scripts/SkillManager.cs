using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages unlocked skills and available throwable items.
/// Skills are unlocked progressively through levels.
/// </summary>
public class SkillManager : MonoBehaviour
{
    [Serializable]
    public class Skill
    {
        public string skillName;
        public int level;
        public bool unlocked;
        public string description;
    }

    [Serializable]
    public class ThrowableEntry
    {
        public string itemName;
        public GameObject prefab;
        public int unlockLevel;
        public bool unlocked;
    }

    [Header("Skills")]
    [SerializeField] private List<Skill> skills = new List<Skill>
    {
        new Skill { skillName = "Basic Bounce", level = 1, unlocked = true, description = "Single bounce to hit target" },
        new Skill { skillName = "Spin Throw", level = 2, unlocked = false, description = "Change bounce angle with spin" },
        new Skill { skillName = "Arc Throw", level = 3, unlocked = false, description = "Over/under throw to clear obstacles" },
        new Skill { skillName = "Sharp Angle Bounce", level = 4, unlocked = false, description = "Throwing knife with sharper rebounds" },
        new Skill { skillName = "Splash Attack", level = 5, unlocked = false, description = "Water balloon with AoE splash damage" },
        new Skill { skillName = "Predictive Throw", level = 6, unlocked = false, description = "Lead moving targets" },
        new Skill { skillName = "AI Dodge Counter", level = 7, unlocked = false, description = "Enemies dodge projectiles" },
    };

    [Header("Throwable Items")]
    [SerializeField] private List<ThrowableEntry> throwableItems = new List<ThrowableEntry>();

    [Header("Current State")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private string currentItemName = "Slipper";

    private Dictionary<string, Skill> skillDict = new Dictionary<string, Skill>();
    private Dictionary<string, ThrowableEntry> itemDict = new Dictionary<string, ThrowableEntry>();

    public int CurrentLevel => currentLevel;
    public string CurrentItemName => currentItemName;

    public event Action<string> OnSkillUnlocked;
    public event Action<string> OnItemUnlocked;
    public event Action<string> OnItemChanged;

    private void Awake()
    {
        // Build dictionaries
        foreach (var skill in skills)
        {
            if (!skillDict.ContainsKey(skill.skillName))
                skillDict[skill.skillName] = skill;
        }

        foreach (var item in throwableItems)
        {
            if (!itemDict.ContainsKey(item.itemName))
                itemDict[item.itemName] = item;
        }
    }

    private void Start()
    {
        // Unlock items for current level
        UpdateUnlocks();
    }

    /// <summary>
    /// Set current level and update unlocks.
    /// </summary>
    public void SetLevel(int level)
    {
        currentLevel = level;
        UpdateUnlocks();
    }

    /// <summary>
    /// Update unlocked skills and items based on current level.
    /// </summary>
    private void UpdateUnlocks()
    {
        foreach (var skill in skills)
        {
            if (skill.level <= currentLevel && !skill.unlocked)
            {
                skill.unlocked = true;
                OnSkillUnlocked?.Invoke(skill.skillName);
                Debug.Log($"Skill unlocked: {skill.skillName}");
            }
        }

        foreach (var item in throwableItems)
        {
            if (item.unlockLevel <= currentLevel && !item.unlocked)
            {
                item.unlocked = true;
                OnItemUnlocked?.Invoke(item.itemName);
                Debug.Log($"Item unlocked: {item.itemName}");
            }
        }
    }

    /// <summary>
    /// Check if a skill is unlocked.
    /// </summary>
    public bool IsSkillUnlocked(string skillName)
    {
        return skillDict.ContainsKey(skillName) && skillDict[skillName].unlocked;
    }

    /// <summary>
    /// Check if an item is unlocked.
    /// </summary>
    public bool IsItemUnlocked(string itemName)
    {
        return itemDict.ContainsKey(itemName) && itemDict[itemName].unlocked;
    }

    /// <summary>
    /// Get the prefab for a throwable item.
    /// </summary>
    public GameObject GetItemPrefab(string itemName)
    {
        if (itemDict.ContainsKey(itemName) && itemDict[itemName].unlocked)
            return itemDict[itemName].prefab;
        return null;
    }

    /// <summary>
    /// Get all unlocked item names.
    /// </summary>
    public List<string> GetUnlockedItemNames()
    {
        List<string> unlocked = new List<string>();
        foreach (var item in throwableItems)
        {
            if (item.unlocked)
                unlocked.Add(item.itemName);
        }
        return unlocked;
    }

    /// <summary>
    /// Switch to a different throwable item.
    /// </summary>
    public void SwitchToItem(string itemName)
    {
        if (IsItemUnlocked(itemName))
        {
            currentItemName = itemName;
            OnItemChanged?.Invoke(itemName);
            Debug.Log($"Switched to item: {itemName}");
        }
        else
        {
            Debug.LogWarning($"Item not unlocked: {itemName}");
        }
    }

    /// <summary>
    /// Get the current throwable item prefab.
    /// </summary>
    public GameObject GetCurrentItemPrefab()
    {
        return GetItemPrefab(currentItemName);
    }
}

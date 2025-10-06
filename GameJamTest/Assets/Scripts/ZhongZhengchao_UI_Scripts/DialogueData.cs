using UnityEngine;
using System.Collections.Generic;

public class DialogueData : MonoBehaviour
{
    public static DialogueData Instance;

    public readonly Dictionary<string, string[]> englishDialogues = new Dictionary<string, string[]>
    {
        // Level 1
        { "Pen", new string[] { "The ink has long dried, but I still remember the letter I never finished." } },
        { "Diary", new string[] { "Some things are easier to write down than to say out loud." } },
        { "Photo", new string[] { "The sunlight was beautiful that day. They were all smiling." } },
        { "Globe", new string[] { "The globe keeps turning, and they've gone so far away." } },
        
        // Level 2
        { "Mirror", new string[] { "The mirror's still here... but that smile's been gone for a long time." } },
        { "Lipstick", new string[] { "That was her favorite lipstick... she never went out without it." } },
        { "JewelryBox", new string[] { "She wore all these little trinkets on our wedding day." } },
        
        // Level 3
        { "CookingPot", new string[] { "My son loved this soup when he was little... I still remember how it tasted." } },
        { "RiceCooker", new string[] { "When the smell of rice fills the air, it still feels like my family's here." } },
        { "SeasoningBottle", new string[] { "Salt, soy sauce, chili... I guess that's what life tastes like." } },
        { "LunchBox", new string[] { "The meal's been cold for a long time... but I still can't bring myself to throw it away." } }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string[] GetDialogue(string itemName)
    {
        if (englishDialogues.ContainsKey(itemName))
            return englishDialogues[itemName];

        Debug.LogWarning($"No dialogue found for: {itemName}");
        return new string[] { "..." };
    }
}
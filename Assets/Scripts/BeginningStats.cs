using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using System.Collections;
using System.Collections.Generic;
using GameManagers;
using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
using UnityEngine;

public enum InitialClasses
{
    MAGE,
    FIGTHER,
    RANGER
}

public class BeginningStats : MonoBehaviour
{
    public GameObject player;
    public Animator animPlayer;
    
    public int healthBase;
    public int healthMax;
    public int magicBase;
    public int magicMax;
    public int attackBase;
    public int magicPowerBase;
    public int defenseBase;
    public int speedBase;
    public float criticalHitProbability;

    [SerializeField] private InitialClasses _initialClass;
    [SerializeField] private List<ItemSlotCollection> _itemCollection;
    [SerializeField] private List<ItemSlotSet> _itemSlotSets;

    public ItemSlotCollectionView slotCollectionView;

    private void Awake()
    {
        if (player != null)
        {
            animPlayer.SetFloat("moveX", 1);
            // player.transform.Rotate(0, 180, 0); //rota la camara y no sirve
        }

        //Inventory startingInventory = gameObject.GetComponent<Inventory>();
        //
        if (GameManager.Instance.inventoryWasInit)
        {
            return;
        }

        var playerInventory = player.GetComponent<Inventory>();

        playerInventory.RemoveItemCollection(_itemCollection[(int)_initialClass]);
        playerInventory.AddItemCollection(_itemCollection[(int)_initialClass]);
        //  startingInventory.AddItemCollection(_itemCollection[(int)_initialClass]);

        playerInventory.RemoveItemCollection(_itemCollection[(int)_initialClass]);
        playerInventory.UpdateInventory();
        //  startingInventory.UpdateInventory();

        if (slotCollectionView == null && GameUIManager.Exist)
        {
            slotCollectionView = GameUIManager.Instance.gameObject.GetComponentInChildren<ItemSlotCollectionView>();
        }

        slotCollectionView.ItemSlotSet = _itemSlotSets[(int)_initialClass];
        slotCollectionView.SetItemViewSlotRestrictions();

        GameManager.Instance.inventoryWasInit = true;

        SetInitialStats(_initialClass);
        
        var playerStats = player.GetComponent<CharStats>();
        
        playerStats.healthBase = healthBase; //  ------------ change playertype 
        playerStats.healthMax = healthMax; //    ------------ change playertype
        playerStats.magicBase = magicBase; //    ------------ change playertype
        playerStats.magicMax = magicMax; //    ------------- change playertype
        playerStats.attackBase = attackBase; // ------------- change playertype
        playerStats.magicPowerBase = magicPowerBase; // ----- change playertype
        playerStats.defenseBase = defenseBase; // ----------- change playertype
        playerStats.speedBase = speedBase; //   ------------ change playertype
        playerStats.criticalHitProbability = criticalHitProbability; //   ------------ change playertype
    }

    private void Start()
    {
        var charStats = player.GetComponent<CharStats>();
        charStats.characterClass = _initialClass.ToString();
    }

    private void SetInitialStats(InitialClasses actualClass)
    {
        switch (actualClass)
        {
            case InitialClasses.FIGTHER:
                SetFighterStats();
                break;
            case InitialClasses.MAGE:
                SetWizardStats();
                break;
            case InitialClasses.RANGER:
                SetRangerStats();
                break;
            // case PlayerType.Barbarian:
            //     SetBarbarianStats();
            //     break;
            default:
                break;
        }
    }


    private void SetFighterStats()
    {
        healthBase = 20;
        healthMax = 100;
        magicBase = 5;
        magicMax = 30;
        attackBase = 5;
        magicPowerBase = 5;
        defenseBase = 7;
        speedBase = 4;
        criticalHitProbability = 0.5f;
    }


    private void SetWizardStats()
    {
        healthBase = 12;
        healthMax = 100;
        magicBase = 25;
        magicMax = 100;
        attackBase = 4;
        magicPowerBase = 1;
        defenseBase = 4;
        speedBase = 5;
        criticalHitProbability = 0.5f;
    }

    private void SetRangerStats()
    {
        healthBase = 15;
        healthMax = 100;
        magicBase = 8;
        magicMax = 40;
        attackBase = 5;
        magicPowerBase = 7;
        defenseBase = 4;
        speedBase = 6;
        criticalHitProbability = 0.6f;
    }

    private void SetBarbarianStats()
    {
    }
}
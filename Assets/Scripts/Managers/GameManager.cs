using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameManagers;
using Newtonsoft.Json;
using Opsive.Shared.Utility;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
using PixelCrushers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Saver
{
    public static GameManager Instance;

    public ItemSlotCollection itemSlotCollection;

    public ItemSlotCollectionView slotCollectionView;

    [SerializeField] private List<ItemSlotCollection> _itemCollection;

    [SerializeField] private InitialClasses _initialClass;

    public bool isLoaded = false;


    public override void ApplyData(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            isLoaded = true;
            return;
        }

        Debug.Log("Data is " + s);
        Debug.Log("apply data");
        var dataBase = JsonConvert.DeserializeObject<List<string>>(s);

        var playerInventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();

        if (!playerInventory) return;

        // var equippedCol = playerInventory.GetItemCollection("Equipped");
        //
        // if (equippedCol != null)
        // {
        //     Debug.Log("Se removio");
        //     playerInventory.RemoveItemCollection(equippedCol);
        // }
        //
        // if (playerInventory == null)
        // {
        //     Debug.Log("es nulo el playerInventory ");
        // }

        // Debug.Log("Se agrego " + itemSlotCollection);
        // playerInventory.AddItemCollection(itemSlotCollection);
        //
        // playerInventory.UpdateInventory();

        //
        // if (slotCollectionView == null || GameUIManager.Exist)
        // {
        //     slotCollectionView = GameUIManager.Instance.gameObject.GetComponentInChildren<ItemSlotCollectionView>();
        //
        //     if (slotCollectionView == null)
        //     {
        //         slotCollectionView = Resources.FindObjectsOfTypeAll<ItemSlotCollectionView>().First();
        //         Debug.Log("Null?");
        //     }
        //
        //     slotCollectionView.ItemSlotSet = itemSlotCollection.ItemSlotSet;
        //
        //     Debug.Log("slotCollectionView.ItemSlotSet" + slotCollectionView.ItemSlotSet);
        //     Debug.Log("itemSlotCollection.ItemSlotSet" + itemSlotCollection.ItemSlotSet);
        // }


        var mainCol = JsonConvert.DeserializeObject<IEnumerable<Tuple<string, int>>>(dataBase[0]);
        playerInventory.GetItemCollection(ItemCollectionPurpose.Main).RemoveAll();

        if (mainCol != null)
        {
            Debug.Log("Main col");
            foreach (var itemInfo in mainCol)
            {
                Debug.Log("Add Item");
                var item = new ItemInfo(itemInfo.Item1, itemInfo.Item2);
                var itemStack = new ItemStack();
                playerInventory.GetItemCollection(ItemCollectionPurpose.Main).AddItem(item, itemStack, false);
                Debug.Log("Finish Item");
            }
            Debug.Log("Finish Main Col");
        }

        playerInventory.GetItemCollection(ItemCollectionPurpose.Equipped).RemoveAll();
        var equipCol = JsonConvert.DeserializeObject<IEnumerable<Tuple<string, int>>>(dataBase[1]);
        if (equipCol != null)
        {
            Debug.Log("Equipped col");
            foreach (var itemInfo in equipCol)
            {
                Debug.Log("Add Equipped item");
                var item = new ItemInfo(itemInfo.Item1, itemInfo.Item2);
                var itemStack = new ItemStack();
                playerInventory.GetItemCollection(ItemCollectionPurpose.Equipped).AddItem(item, itemStack, false);
                Debug.Log("Finish Equipped item");
            }
            Debug.Log("Finish Equipped Col");
        }

        Debug.Log("Is Loaded");
        //Debug.Log(playerInventory.GetItemCollection("Equipped"));
        isLoaded = true;
    }

    public override string RecordData()
    {
        var DataList = new List<string>();
        var playerInventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();

        if (playerInventory != null)
        {
            var dataItems =
                new ItemInfo[playerInventory.GetItemCollection(ItemCollectionPurpose.Main).GetAllItemStacks().Count];
            playerInventory.GetItemCollection(ItemCollectionPurpose.Main).GetAllItemInfos(ref dataItems);
            var itemAmount = dataItems.Select(x => Tuple.Create(x.Item.name, x.Amount));
            DataList.Add(JsonConvert.SerializeObject(itemAmount));

            var equippedDataItem =
                new ItemInfo[playerInventory.GetItemCollection(ItemCollectionPurpose.Equipped).GetAllItemStacks()
                    .Count];
            playerInventory.GetItemCollection(ItemCollectionPurpose.Equipped).GetAllItemInfos(ref equippedDataItem);
            var equippedItemAmount = equippedDataItem.Select(x => Tuple.Create(x.Item.name, x.Amount));
            DataList.Add(JsonConvert.SerializeObject(equippedItemAmount));
        }
        else
        {
            DataList.Add(string.Empty);
            DataList.Add(string.Empty);
        }

        DataList.Add(JsonConvert.SerializeObject(_initialClass));
        return JsonConvert.SerializeObject(DataList);
    }

    public override void Awake()
    {
        Debug.Log("Start GameManager Awake");
        if (Instance)
        {
            Destroy(gameObject);
            Debug.Log("Destroying game manager...");
            return;
        }

        isLoaded = false;
        Debug.Log("awake");
        
        // SaveSystem.RegisterSaver(this);

        string charClass = ICConnect.characterClass;

        VerifyClassICConnect(charClass);

        itemSlotCollection = _itemCollection[(int)_initialClass];

        Instance = this;

        var playerInventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();

        if (!playerInventory) return;

        var equippedCol = playerInventory.GetItemCollection(ItemCollectionPurpose.Equipped);

        if (equippedCol != null)
        {
            playerInventory.RemoveItemCollection(equippedCol);
        }

        if (playerInventory == null)
        {
            Debug.Log("es nulo el playerInventory ");
        }

        playerInventory.AddItemCollection(itemSlotCollection);

        playerInventory.UpdateInventory();


        if (slotCollectionView == null || GameUIManager.Exist)
        {
            slotCollectionView = GameUIManager.Instance.gameObject.GetComponentInChildren<ItemSlotCollectionView>();

            if (slotCollectionView == null)
            {
                slotCollectionView = Resources.FindObjectsOfTypeAll<ItemSlotCollectionView>().First();
                Debug.Log("Null?");
            }

            slotCollectionView.ItemSlotSet = itemSlotCollection.ItemSlotSet;

            Debug.Log("slotCollectionView.ItemSlotSet" + slotCollectionView.ItemSlotSet);
            Debug.Log("itemSlotCollection.ItemSlotSet" + itemSlotCollection.ItemSlotSet);
        }

        // SaveChecker.Instance.HasToLoad = true;
        isLoaded = true;
        Debug.Log(playerInventory.GetItemCollection("Equipped"));
        Debug.Log("Finish GameManager Awake");
    }

    // public override void OnDestroy()
    // {
    //     SaveSystem.UnregisterSaver(this);
    // }

    private void VerifyClassICConnect(string charClass)
    {
        if (charClass == "MAGE")
        {
            _initialClass = InitialClasses.MAGE;
        }

        if (charClass == "FIGHTER")
        {
            _initialClass = InitialClasses.FIGHTER;
        }

        if (charClass == "RANGER")
        {
            _initialClass = InitialClasses.RANGER;
        }

        if (charClass == "Default" || charClass == null)
        {
            _initialClass = InitialClasses.RANGER;
        }
    }
}
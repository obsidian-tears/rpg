using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Opsive.UltimateInventorySystem.Core;
using GameManagers;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;

public enum PlayerType { Fighter, Wizard, Rogue, Barbarian, Ranger }
public class Player : MonoBehaviour
{

    [HideInInspector] [SerializeField] public float speed = 3f;
    private float speedMultiplier = 1.0f;
    public Rigidbody2D myRigidbody;
    public Vector3 change;
    public Animator animator;
    public FloatValue maxHealth;
    public FloatValue currentHealth;
    public FloatValue maxMagic;
    public FloatValue currentMagic;
    [Space(5)]
    [Header("Global game context reference")]
    [SerializeField] GlobalGameContextSORS m_globalGameContext;
    [Space(5)]
    [Header("Signals")]
    [SerializeField] MySignal playerHealthSignal;
    [SerializeField] MySignal battleSignal;
    [SerializeField] MySignal dialogSignal;
    /*Note from Isaac: I've removed the playerPosition object from this script and all its references (I just commented it out) to replace its use with the save system I've implemented.
     * The player instead has a PositionSaver component that saves the player's position on scene changes and when the game is actually saved/exited. This avoids having to constantly
     * change player position in Update calls*/
    //[SerializeField] VectorValue playerPosition;
    [SerializeField] GameObject pauseMenu;
    //[SerializeField] PlayerType playerType = PlayerType.Fighter;
    [SerializeField] bool frozen;

    [SerializeField] private bool isRunning;

    void Awake() {
        if (m_globalGameContext != null)
        {
            m_globalGameContext.RegisterPlayerObject(this);
        }
        else 
        {
            Debug.LogError("Cannot find attached global game context!", gameObject);
        }

        //Register player as the inventory panel owner and register inventory, currency monitor(s)
        InventorySystemManager.GetDisplayPanelManager().SetPanelOwner(gameObject);
        Inventory inventory = GetComponent<Inventory>();
        if (inventory != null)
        {
            GameUIManager.Instance.SetInventory(inventory);
            GameUIManager.Instance.SetCurrencyOwner(inventory.GetCurrencyComponent<CurrencyCollection>() as CurrencyOwner);
        }
        else
        {
            Debug.LogError("FATAL ERROR! Player has no inventory component! Please assign one!");
        }

        frozen = false;
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHealthSignal.Raise();
    }

    // Update is called once per frame
    void Update()
    {
        if (frozen) return;
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        if (CheckMobile.IsMobile) 
        { 
             speedMultiplier =  isRunning ? 1.5f : 1.0f; // Modificado       
        
        }
        else
        {
            speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1.0f;

        }

        change = change.normalized * speedMultiplier;
        //playerPosition.initialValue.x = transform.position.x;
        //playerPosition.initialValue.y = transform.position.y;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (pauseMenu.activeInHierarchy)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
            }
        }

    }

    void FixedUpdate()
    {
        if (frozen) {
            animator.SetBool("moving", false);
            return;
        }
        if (change != Vector3.zero)
        {
            myRigidbody.MovePosition(transform.position + change * speed * Time.deltaTime);
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    public void OnMonsterSignal()
    {
        if (animator.GetBool("moving"))
        {
            frozen = true;
            // check for potion effects, magic armor, etc. 
            battleSignal.Raise();
        }
    }

    public void OnDialog() {
        if (!frozen) {
            dialogSignal.Raise();
        }
    }

    public void Freeze() {
        frozen = true;
    }
    public void Unfreeze() {
        frozen = false;
    }


    public void HandleRunButtonPressed()
    {
        isRunning = true;
    }

    public void HandleRunButtonReleased() 
    {
        isRunning = false;
    }
}

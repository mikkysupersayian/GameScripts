using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacteristics : MonoBehaviour
{
    [SerializeField]
    private int playerLevel;
    [SerializeField]
    private int playerEXP;
    [SerializeField]
    private float playerHealth;
    [SerializeField]
    private float playerMana;

    public Text playerLevel_text;
    public Text playerEXP_text;

    public int pointsAvailable;

    public Text pointsAvailable_text;

    public int playerInventorySize;
    public GameObject[] itemInventory;
    public GameObject inventoryController;

    [SerializeField]
    private int playerInventoryTaken;

    [SerializeField]
    private int nextToLevelUp;

    public int playerMoney;
    public Text playerMoney_text;

    private void Awake()
    {
        itemInventory = new GameObject[playerInventorySize];

        checkInventory();

        playerLevel_text.text = playerLevel.ToString();

        nextToLevelUp = playerLevel * 100;

        pointsAvailable_text.text = pointsAvailable.ToString();
    }

    /*
     * Logical functions for Player Level and if Player Experience is being gained
     */
    private void Update()
    {
        if(Int32.Parse(playerLevel_text.text) != playerLevel)
        {
            playerLevel_text.text = playerLevel.ToString();
        }

        if(Int32.Parse(playerMoney_text.text) != playerMoney)
        {
            playerMoney_text.text = playerMoney.ToString();
        }

        if(playerEXP >= nextToLevelUp)
        {
            playerLevel++;
            pointsAvailable++;

            pointsAvailable_text.text = pointsAvailable.ToString();

            int temp = playerEXP;

            if (temp > nextToLevelUp)
            {
                playerEXP = 0;

                temp -= nextToLevelUp;

                playerEXP += temp;
            }

            nextToLevelUp = 0;
            nextToLevelUp = playerLevel * 100;
        }
    }

    /*
     * Add item to the inventory, othersie debug error is thrown
     */
    public void addItem(GameObject item)
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] == null)
            {
                itemInventory[i] = item;
                item.SetActive(false);
                checkInventory();

                inventoryController.GetComponent<InventoryController>().addItem(item.GetComponent<ItemInfo>().itemData);

                break;
            } else
            {
                Debug.Log("Inventory full");
            }
        }
    }

    /*
     * Check inventory of the player, PlayerInventoryTaken is updated based on available inventory size
     */
    public void checkInventory()
    {
        playerInventoryTaken = 0;

        for (int i = 0; i < playerInventorySize; i++)
        {
            if (itemInventory[i] != null)
            {
                playerInventoryTaken++;
            }
        }
    }

    /*
     * Check if item can be bought based on inventory, based on inventory size.
	 * Returns 1 if available, otherwise 0
     */
    public int checkToBuy()
    {
        int available = 0;

        for(int i = 0; i < playerInventorySize; i++)
        {
            if(itemInventory[i] == null)
            {
                available++;
            }
        }

        if(available > 0)
        {
            return 1;
        } else
        {
            return 0;
        }
    }

    public int PlayerInventoryTaken
    {
        get
        {
            return playerInventoryTaken;
        }
    }

    public void addExp(int amount)
    {
        this.playerEXP += amount;
    }

    public void removeItem(int index)
    {
        itemInventory[index] = null;
    }
}
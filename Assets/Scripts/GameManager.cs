using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private void Awake()
    {
        if(GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hub);
            Destroy(menu);
            return;
        }

        PlayerPrefs.DeleteAll();

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;


    }

    //Ressources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;

    //References
    public Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public RectTransform hitpointBar;
    public Animator deathMenuAnim;
    public GameObject hub;
    public GameObject menu;

    //Logic
    public int coins;
    public int experience;

    public void ShowText(string msg, int fontSize, Color color, Vector3 potision, Vector3 motion, float duration )
    {
        floatingTextManager.Show(msg, fontSize, color, potision, motion, duration);
    }

    //Upgrade Weapon
    public bool TryUpgradeWeapon()
    {
        // is the weapon max level?
        if (weaponPrices.Count <= weapon.weaponLevel)
        {
            return false;
        }
        if(coins >= weaponPrices[weapon.weaponLevel])
        {
            coins -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }
        return false;
    }

    // Hitpoint Bar
    public void OnHitpointChange()
    {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }

    // Experience System
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while (experience >= add)
        {
            add += xpTable[r];
            r++;

            if(r == xpTable.Count) // Max level
            {
                return r;
            }
        }
        return r;
    }
    public int GetXpToLevel(int level)
    {
        int r = 0;
        int xp = 0;

        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }
        return xp;
    }
    public void GrantXp(int xp)
    {
        int currLevel = GetCurrentLevel();
        experience += xp;
        if(currLevel < GetCurrentLevel())
        {
            OnLevelUp();
        }
    }
    public void OnLevelUp()
    {
        Debug.Log("Level Up");
        player.OnLevelUp();
        OnHitpointChange();
    }

    // On Scene Loaded
    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }

    // Death Menu and Respawn
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        player.Respawn();
    }

    //Save state
    public void SaveState()
    {
        string s = "";

        s += "0" + "|";
        s += coins.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString();

        PlayerPrefs.SetString("SaveState", s);
    }
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded += LoadState;

        if (PlayerPrefs.HasKey("SaveState"))
        {
            return;
        }

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        // Change palyer skin
        coins = int.Parse(data[1]);

        // Exp
        experience = int.Parse(data[2]);
        if(GetCurrentLevel() != 1)
        player.SetLevel(GetCurrentLevel());

        // Change the weapon Level
        weapon.SetWeaponLevel(int.Parse(data[3]));
        
    }


}

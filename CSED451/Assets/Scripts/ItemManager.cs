using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemManager : MonoBehaviour
{
    // Item Code
    // Coin=0, Shell=1, Potion=2, Mushroom=3
    public string[] itemList = {"ItemCoin", "ItemShell", "ItemPotion", "ItemMushroom"};
    public int[] numItem = {5, 5, 5, 5};
    public int currentItem = 0;
    public float coolDown = 2.0f;
    public KeyCode useKey = KeyCode.Mouse0;
    public TextMeshProUGUI coinNum;
    public TextMeshProUGUI shellNum;
    public TextMeshProUGUI potionNum;
    public TextMeshProUGUI mushroomNum;
    public GameObject coinPanel;
    public GameObject shellPanel;
    public GameObject potionPanel;
    public GameObject mushroomPanel;
    public Slider powerSlider;
    public GameObject powerGage;
    public Camera cam;
    public Transform player;
    public AudioClip itemGetAudio;

    float m_coolTime = 0;
    bool m_ready;
    bool m_isCharging;
    float m_chargePower;
    Throw m_throwScript;
    //ddd m_potionScript;
    Mushroom m_mushroomScript;
    PlayerTransperent m_potionScript;
    PlayerWardrobe m_playerWardrobe;
    AudioSource m_audioSource;

    readonly KeyCode[] m_keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };

    // Start is called before the first frame update
    void Start()
    {
        m_ready = true;
        m_isCharging = false;
        m_chargePower = 0f;
        m_throwScript = GetComponent<Throw>();
        m_potionScript = GetComponent<PlayerTransperent>();
        //m_potionScript = GetComponent<TransperentPotion>();
        m_mushroomScript = GetComponent<Mushroom>();
        m_playerWardrobe = GetComponent<PlayerWardrobe>();
        m_audioSource = GetComponent<AudioSource>();

        coinNum.text = "x 00";
        shellNum.text = "x 00";
        potionNum.text = "x 00";
        mushroomNum.text = "x 00";

        powerGage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //UI Update
        SetItemNum();
        if (m_isCharging) {
            Charging();
        }
        if (m_playerWardrobe.isPlayerInWardrobe) {
            StopCharging();
        }
        /*
        if (!m_ready)
        {
            m_coolTime += Time.deltaTime;
            coinPanel.rectTransform.localScale = new Vector3(m_coolTime / 2.0f, 1.0f, 1.0f);
            shellPanel.rectTransform.localScale = new Vector3(m_coolTime / 2.0f, 1.0f, 1.0f);
            potionPanel.rectTransform.localScale = new Vector3(m_coolTime / 2.0f, 1.0f, 1.0f);
            mushroomPanel.rectTransform.localScale = new Vector3(m_coolTime / 2.0f, 1.0f, 1.0f);
        }
        else
        {
            m_coolTime = 0;
        }
        */
        if (currentItem == 0)
        {
            coinPanel.SetActive(true);
            shellPanel.SetActive(false);
            potionPanel.SetActive(false);
            mushroomPanel.SetActive(false);
        }
        else if (currentItem == 1)
        {
            coinPanel.SetActive(false);
            shellPanel.SetActive(true);
            potionPanel.SetActive(false);
            mushroomPanel.SetActive(false);
        }
        else if (currentItem == 2)
        {
            coinPanel.SetActive(false);
            shellPanel.SetActive(false);
            potionPanel.SetActive(true);
            mushroomPanel.SetActive(false);
        }
        else if (currentItem == 3)
        {
            coinPanel.SetActive(false);
            shellPanel.SetActive(false);
            potionPanel.SetActive(false);
            mushroomPanel.SetActive(true);
        }

        // Use Item or Charge Item
        powerSlider.transform.position = cam.WorldToScreenPoint(player.position + new Vector3(0.7f, 0.5f, 0));
        powerSlider.value = m_chargePower;
        if (Input.GetKeyDown(useKey) && m_ready && numItem[currentItem] > 0 && !m_playerWardrobe.isPlayerInWardrobe)
        {
            if (currentItem == 0 || currentItem == 1) {
                StartCharging();
            }
            else if (currentItem == 2) {
                Use(currentItem);
            }
            else if (currentItem == 3)
            {
                Use(currentItem);
            }
        }
        else if (Input.GetKeyUp(useKey) && !m_playerWardrobe.isPlayerInWardrobe)
        {
            if (currentItem == 0 || currentItem == 1) {
                if (m_isCharging) Use(currentItem);
                StopCharging();
            }
        }

        // Change currently chosen item
        for (int i = 0 ; i < m_keyCodes.Length; i ++ )
        {
            if (Input.GetKeyDown(m_keyCodes[i]))
            {
                ChangeItem(i);
            }
        }
    }

    void GetItem(int itemCode)
    {
        numItem[itemCode]++;
    }

    void ChangeItem(int itemCode)
    {
        if (m_isCharging) {
            StopCharging();
        }
        currentItem = itemCode;
    }

    void StartCharging()
    {
        m_chargePower = 0;
        m_isCharging = true;
        powerGage.SetActive(true);
    }

    void Charging()
    {
        m_chargePower += Time.deltaTime;
        m_chargePower = Mathf.Clamp(m_chargePower, 0, 1);
    }

    void StopCharging()
    {
        m_chargePower = 0;
        m_isCharging = false;
        powerGage.SetActive(false);
    }

    void Use(int itemCode)
    {
        m_ready = false;
        if (itemCode == 0 || itemCode == 1)
        {
            m_throwScript.Use(m_chargePower, itemCode);
        }
        else if (itemCode == 2)
        {
            m_potionScript.Use();
        }
        else if (itemCode == 3)
        {
            m_mushroomScript.Use();
        }
        numItem[itemCode]--;
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDown);
        m_ready = true;
    }

    void OnTriggerEnter(Collider col)
    {
        for (int i = 0; i < itemList.Length; i++)
        {
            if (col.gameObject.CompareTag(itemList[i]))
            {
                GetItem(i);
                m_audioSource.clip = itemGetAudio;
                m_audioSource.Play();
                Destroy(col.gameObject);
            }
        }
    }

    void SetItemNum()
    {
        coinNum.text = "x " + (numItem[0] / 10).ToString() + (numItem[0] % 10).ToString();
        shellNum.text = "x " + (numItem[1] / 10).ToString() + (numItem[1] % 10).ToString();
        potionNum.text = "x " + (numItem[2] / 10).ToString() + (numItem[2] % 10).ToString();
        mushroomNum.text = "x " + (numItem[3] / 10).ToString() + (numItem[3] % 10).ToString();
    }
}

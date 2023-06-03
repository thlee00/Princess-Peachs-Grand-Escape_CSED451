using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // Item Code
    // Coin=0, Shell=1, Potion=2
    public string[] itemList = {"Coin", "Shell", "Potion"};
    public int[] numItem = {5, 5, 5};
    public int currentItem = 0;
    public float coolDown = 2.0f;
    public KeyCode useKey = KeyCode.Mouse0;
    bool m_ready;
    bool m_isCharging;
    float m_chargePower;
    Throw m_throwScript;
    //TH : Add potion use script like above
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
        // TH : GetComponent<Potion Script>()
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isCharging) {
            Charging();
        }

        // Use Item or Charge Item
        if (Input.GetKeyDown(useKey) && m_ready && numItem[currentItem] > 0)
        {
            if (currentItem == 0 || currentItem == 1) {
                StartCharging();
            }
            else if (currentItem == 2) {
                Use(currentItem);
            }
        }
        else if (Input.GetKeyUp(useKey))
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
    }


    void Use(int itemCode)
    {
        m_ready = false;
        if (itemCode == 0 || itemCode == 1)
        {
            m_throwScript.Use(m_chargePower);
        }
        else if (itemCode == 2)
        {
            // TH : call potion use method
        }
        numItem[itemCode]--;
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDown);
        m_ready = true;
    }
}

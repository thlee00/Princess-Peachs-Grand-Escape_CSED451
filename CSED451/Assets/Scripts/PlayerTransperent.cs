using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransperent : MonoBehaviour
{
    public float transperentDelay = 5.0f;
    public bool isPlayerTransperented;

    List<Renderer> renderers;
    int numOfChild;

    // Start is called before the first frame update
    void Start()
    {
        renderers = new List<Renderer>();
        numOfChild = transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            Renderer temp = transform.GetChild(i).GetComponent<Renderer>();
            if (temp != null)
            {
                renderers.Add(temp);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Use()
    {
        isPlayerTransperented = true;
        StartCoroutine(startTransperent());
    }

    IEnumerator startTransperent()
    {
        print("startTransperent");
        print(renderers.Count);
        for (int r = 0; r < renderers.Count; r++)
        {
            print(renderers[1]);
            Color c = renderers[1].material.color;
            print(c);
            c.a = c.a / 10;
            renderers[1].material.color = c;
            print(c);
        }
        yield return new WaitForSeconds(transperentDelay);
        isPlayerTransperented = false;
        print("endTransperent");
    }
}

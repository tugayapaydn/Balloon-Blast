using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPanel : MonoBehaviour
{
    public List<Sprite> heroSprites = new List<Sprite>();
    private List<GameObject> children = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
            transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = heroSprites[Random.Range(0, heroSprites.Count)];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

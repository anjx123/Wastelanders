using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    //Getter for the Singleton is found here
    public static CombatManager Instance { get; private set; }

    // Priority Queue
    public List<EntityClass> players;
    public List<EntityClass> enemies;

    public GameObject instance; // Assign your instantiated object in the Inspector
    public GameObject container; // Assign your canvas in the Inspector

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < 1; i++)
        //{
        //    GameObject pee = Instantiate(instance);
        //    pee.transform.SetParent(container.transform, false);
        //}
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}

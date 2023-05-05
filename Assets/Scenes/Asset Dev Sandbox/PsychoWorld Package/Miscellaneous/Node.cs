using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> prefabList = new();
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    

    void Start()
    {
        prefabList.Add(prefab1);
        prefabList.Add(prefab2);
        prefabList.Add(prefab3);
       
        int prefabIndex = Random.Range(0,prefabList.Count-1);
        GameObject prefab = Instantiate(prefabList[prefabIndex], transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

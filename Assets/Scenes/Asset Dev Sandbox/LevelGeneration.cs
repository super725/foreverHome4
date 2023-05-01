using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    [Header("Level Properties")] 
    public int mapWidthInTiles;
    public int mapDepthInTiles;

    [Header("Prefab List")] 
    public GameObject[] prefabs;
        
    [Header("Texture Properties")]
    public GameObject tilePrefab;
    public int tileResolution;
    public Texture2D texture1, texture2;
    public AnimationCurve blendCurve;
    
    [Header("Height Map Perlin Noise")]
    public float levelScale;
    public float heightMultiplier;
    public AnimationCurve heightCurve;
    public Wave[] heightWave, scatterWave;
    
    [Header("Scatter Map Properties")] 
    public float scatterDensity;
    public float scatterRadius;
    
    [Header("Miscellaneous")]
    public GameObject playerPrefab;

    private Transform myTransform;
    private Vector3 myPosition;
    
    [NonSerialized]
    public int tileWidth, tileDepth;

    void Start()
    {
        // get the tile dimensions from the tile Prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        tileWidth = (int)tileSize.x;
        tileDepth = (int)tileSize.z;
        
        myTransform = transform;
        myPosition = myTransform.position;
        StartCoroutine(nameof(GenerateMap));
    }

    IEnumerator GenerateMap() {
        // for each Tile, instantiate a Tile in the correct position
        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++) {
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++) {
                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(myPosition.x + xTileIndex * tileWidth, myPosition.y, myPosition.z + zTileIndex * tileDepth);
                Instantiate (tilePrefab, tilePosition, Quaternion.identity, myTransform);
            }
        }
        yield return 0; // Raycast collision doesnt work until after the mesh generation frame
        

        Vector2 centerPos = new(mapWidthInTiles * tileWidth / 2f, mapDepthInTiles * tileDepth / 2f);
        InstantiateObjectInMap(playerPrefab, centerPos.x, centerPos.y);
    }
    
    private void InstantiateObjectInMap(GameObject prefab, float x, float y)
    {
        Vector3 pos = transform.position;
        Vector3 prefabPos = new(pos.x + x - tileWidth/2f + 0.5f, pos.y + 10, pos.z + y - tileDepth/2f + 0.5f);
        Ray ray = new(prefabPos, Vector3.down);
        if (Physics.Raycast(ray, out var hit, 15f))
        {
            prefabPos = hit.point + Vector3.up;
        }
        Instantiate(prefab, prefabPos, Quaternion.identity);
    }
}
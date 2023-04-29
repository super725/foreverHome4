using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    [Header("Level Properties")] 
    public int mapWidthInTiles;
    public int mapDepthInTiles;
        
    [Header("Texture Properties")]
    public GameObject tilePrefab;
    public int tileResolution;
    public Texture2D texture1, texture2;
    public AnimationCurve blendCurve;
    
    [Header("HeightMap Perlin Noise")]
    public float levelScale;
    public float heightMultiplier;
    public AnimationCurve heightCurve;
    public Wave[] heightWave, scatterWave;
    
    [Header("ScatterMap Properties")] 
    public float scatterDensity;
    public float scatterRadius;

    [Header("Miscellaneous")]
    public GameObject playerPrefab;
    
    private Transform myTransform;
    private Vector3 myPosition;
    private int tileWidth, tileDepth;

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
        
        Vector3 playerPos = new Vector3(myPosition.x + mapWidthInTiles * tileWidth / 2f - tileWidth/2f, myPosition.y + 10, myPosition.z + mapDepthInTiles * tileDepth / 2f  - tileDepth/2f);
        RaycastHit hit;
        Ray ray = new(playerPos, Vector3.down);

        if (Physics.Raycast(ray, out hit, 15f))
        {
            Debug.Log(hit.collider.name);
            Debug.DrawRay(playerPos, hit.point-playerPos, Color.red, 5f);
            playerPos = hit.point + Vector3.up;
        }
        
        Instantiate(playerPrefab, playerPos, Quaternion.identity);
    }
}
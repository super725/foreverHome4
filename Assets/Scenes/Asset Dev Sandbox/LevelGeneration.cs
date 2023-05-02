using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    [Header("Level Properties")] 
    public int rawMapWidth;
    public int rawMapDepth;
    public int borderThickness;
    
    
    [Header("Prefab List")] 
    public Prefab[] prefabs;
        
    [Header("Texture Properties")]
    public GameObject tilePrefab;
    public GameObject flatPrefab;
    public GameObject borderPrefab;
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
    
    [NonSerialized] public int tileWidth, tileDepth;
    [NonSerialized] public int mapWidth, mapDepth;
    void Start()
    {
        if (borderThickness < 2)
        {
            borderThickness = 2;
        }
        mapWidth = rawMapWidth + borderThickness * 2;
        mapDepth = rawMapDepth + borderThickness * 2;
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
        for (int xTileIndex = 0; xTileIndex < mapWidth; xTileIndex++) {
            for (int zTileIndex = 0; zTileIndex < mapDepth; zTileIndex++) {
                // EX. 2x2 playable, 4x4 border, 6x6flat tiles
                // raw would be 2x2
                // border thickness would be 2
                // map width would be 6
                
                //bounds of flat would be noninclusive map-thickness = 6-2
                
                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(myPosition.x + xTileIndex * tileWidth, myPosition.y, myPosition.z + zTileIndex * tileDepth);
                
                //  0-1 and 5-6
                if((xTileIndex >= 0 && xTileIndex < borderThickness-1 || xTileIndex > mapWidth-borderThickness && xTileIndex <= mapWidth)
                   || (zTileIndex >= 0 && zTileIndex < borderThickness-1 || zTileIndex > mapDepth-borderThickness && zTileIndex <= mapDepth)
                   )
                {
                    // Flat
                    Instantiate (flatPrefab, tilePosition, Quaternion.identity, myTransform);
                }
                // 2 and 4
                else if((xTileIndex == borderThickness-1 || xTileIndex == mapWidth-borderThickness )
                        || (zTileIndex == borderThickness-1 || zTileIndex == mapDepth-borderThickness))
                {
                    // Border
                    Instantiate (borderPrefab, tilePosition, Quaternion.identity, myTransform);
                }
                else
                {
                    Instantiate (tilePrefab, tilePosition, Quaternion.identity, myTransform);
                }
            }
        }
        
        yield return 0; // Raycast collision doesnt work until after the mesh generation frame
        
        Vector2 centerPos = new(mapWidth * tileWidth / 2f, mapDepth * tileDepth / 2f);
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

[Serializable]
public class Prefab {
    public GameObject prefab;
    public bool useScaleCurve;
    public AnimationCurve scale;
}
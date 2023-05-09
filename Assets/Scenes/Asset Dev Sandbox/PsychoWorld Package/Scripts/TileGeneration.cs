using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class TileGeneration : MonoBehaviour
{
	private int tileResolution;
	private Texture2D texture1;
	private Texture2D texture2;
	private AnimationCurve blendCurve;
	
	private float levelScale;
	private float heightMultiplier;
	private AnimationCurve heightCurve;
	private Wave[] heightWave, scatterWave;
	
	public NoiseMapGeneration noiseMapGeneration;

	private bool isHome;
	private Prefab[] staticPrefabs;
	private Prefab[] dynamicPrefabs;
	private Prefab[] psychoPrefabs;

	//public GameObject treePrefab, spherePrefab;
	
	[SerializeField] private MeshRenderer tileRenderer;
	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private MeshCollider meshCollider;

	private float mapWidth, mapDepth, tileWidth, tileDepth, scatterDensity, scatterRadius;
	private static readonly int Metallic = Shader.PropertyToID("_Metallic");
	private static readonly int Glossiness = Shader.PropertyToID("_Glossiness");

	void Start()
	{
		LevelGeneration args = GetComponentInParent<LevelGeneration>();
		tileResolution = args.tileResolution;
		texture1 = args.texture1;
		texture2 = args.texture2;
		blendCurve = args.blendCurve;
		levelScale = args.levelScale;
		heightMultiplier = args.heightMultiplier;
		heightCurve = args.heightCurve;
		heightWave = args.heightWave;
		scatterWave = args.scatterWave;
		scatterRadius = args.scatterRadius;
		scatterDensity = args.scatterDensity;
		staticPrefabs = args.staticPrefabs;
		dynamicPrefabs = args.dynamicPrefabs;
		psychoPrefabs = args.psychoPrefabs;
		isHome = args.isHome;

		Vector3 tileSize = GetComponent<MeshRenderer>().bounds.size;
		tileWidth = (int)tileSize.x;
		tileDepth = (int)tileSize.z;

		mapWidth = args.mapWidth;
		mapDepth = args.mapDepth;
		
		StartCoroutine(nameof(GenerateTile));
	}

	IEnumerator GenerateTile() {
		// Offset tiles based on position
		var position = transform.position;
		var offsetX = -position.x;
		var offsetZ = -position.z;

		// Use Perlin Noise to generate a height map
		float[,] heightMap = GenerateHeightMap(offsetX, offsetZ);
		UpdateMeshVertices(heightMap); // Use new heightmap as our vertices' y values.
		
		yield return 0;

		// Place critical static terrain prefabs
		for (int i = 0; i < staticPrefabs.Length; i++)
		{
			NonParityScatter(staticPrefabs[i], scatterDensity);
		}
		
		// Place the shroomy stuff if in psycho
		if (!isHome)
		{
			for (int i = 0; i < psychoPrefabs.Length; i++)
			{
				NonParityScatter(psychoPrefabs[i], scatterDensity);
			}
		}

		
		yield return 0;
		
		// Do this after Static prefab loading, but before dynamic prefab loading
		BuildNavMesh(gameObject);
		
		yield return 0;
		
		// Place animate/dynamic prefabs
		for (int i = 0; i < dynamicPrefabs.Length; i++)
		{
			float[,] generativeMap = GenerateScatterMap(offsetX+i, offsetZ+i);
			ParityScatter(dynamicPrefabs[i], generativeMap, scatterRadius);
		}
		
		// build a Texture2D from the height map
		Texture2D tileTexture = BuildTexture (heightMap);
		tileRenderer.material.mainTexture = tileTexture;
		tileRenderer.material.SetFloat(Metallic,0.05f);
		tileRenderer.material.SetFloat(Glossiness,0.05f);
	}

	private float[,] GenerateHeightMap(float offsetX, float offsetZ) {
		// Use our meshFilter vertices to figure out how the dimensions of our heightmap
		Vector3[] meshVertices = meshFilter.mesh.vertices;
		int tileMeshDepth = (int)Mathf.Sqrt (meshVertices.Length);
		int tileMeshWidth = tileMeshDepth;
		
		return noiseMapGeneration.GenerateNoiseMap (tileMeshDepth, tileMeshWidth, levelScale, offsetX, offsetZ, heightWave);
	}

	private float[,] GenerateScatterMap(float offsetX, float offsetZ) {
		// Use our meshFilter vertices to figure out how the bounds of our scatterings
		int tileMeshDepth = (int)GetComponent<MeshRenderer>().bounds.size.z;
		int tileMeshWidth = (int)GetComponent<MeshRenderer>().bounds.size.x;
		
		return noiseMapGeneration.GenerateNoiseMap (tileMeshDepth, tileMeshWidth, levelScale, offsetX, offsetZ, scatterWave);
	}
	
	private Texture2D BuildTexture(float[,] heightMap) {
		// Height map resolution is mesh vertex resolution
		// Texture resolution will be different from heightmap resolution
		// Scaling needed.

		int heightMapWidth = (int)Mathf.Sqrt(heightMap.Length);
		int textureWidth = Mathf.Min(tileResolution, texture1.width);
		
		float textureScaleFactor = (float)texture1.width / textureWidth;
		float heightScaleFactor = (float)textureWidth / heightMapWidth;
		
		float[,] hiResHeight = UpSampleFloat2DArray(heightMap, heightScaleFactor);

		// Debug.Log($"Original Texture Resolution: {texture1.width}");
		// Debug.Log($"Original Heightmap Resolution: {heightMapWidth}");
		// Debug.Log($"Selected Texture Resolution: {textureWidth}");
		// Debug.Log($"Height Map -> Texture Scale Factor: {heightScaleFactor}");
		// Debug.Log($"Texture Down-sampling Factor: {textureSaleFactor}");

		Color[] colorMap = new Color[textureWidth * textureWidth];

		for (int zIndex = 0; zIndex < textureWidth; zIndex++) {
			for (int xIndex = 0; xIndex < textureWidth; xIndex++) {
				// transform the 2D map index is an Array index
				int colorIndex = zIndex * textureWidth + xIndex;

				float heightWeight = blendCurve.Evaluate(hiResHeight[zIndex, xIndex]);
				Vector2Int scaledUV = new((int)(zIndex * textureScaleFactor), (int)(xIndex * textureScaleFactor));
				
				colorMap[colorIndex] =  Color.Lerp(texture2.GetPixel(scaledUV.x, scaledUV.y), texture1.GetPixel(scaledUV.x, scaledUV.y), heightWeight);
			}
		}

		// create a new texture and set its pixel colors
		Texture2D tileTexture = new Texture2D (textureWidth, textureWidth)
		{
			wrapMode = TextureWrapMode.Repeat
		};
		tileTexture.SetPixels (colorMap);
		tileTexture.Apply();

		return tileTexture;
	}
	
	private static float[,] UpSampleFloat2DArray(float[,] inputArray, float scaleFactor)
	{
		int inputWidth = inputArray.GetLength(0);
		int inputHeight = inputArray.GetLength(1);

		int outputWidth = Mathf.RoundToInt(inputWidth * scaleFactor);
		int outputHeight = Mathf.RoundToInt(inputHeight * scaleFactor);

		float[,] outputArray = new float[outputWidth, outputHeight];

		for (int y = 0; y < outputHeight; y++)
		{
			for (int x = 0; x < outputWidth; x++)
			{
				float sourceX = x / scaleFactor;
				float sourceY = y / scaleFactor;

				int x0 = Mathf.FloorToInt(sourceX);
				int x1 = Mathf.Min(x0 + 1, inputWidth - 1);
				int y0 = Mathf.FloorToInt(sourceY);
				int y1 = Mathf.Min(y0 + 1, inputHeight - 1);

				float fx = sourceX - x0;
				float fy = sourceY - y0;

				float a = inputArray[x0, y0];
				float b = inputArray[x1, y0];
				float c = inputArray[x0, y1];
				float d = inputArray[x1, y1];

				float value = Mathf.Lerp(Mathf.Lerp(a, b, fx), Mathf.Lerp(c, d, fx), fy);
				outputArray[x, y] = value;
			}
		}

		return outputArray;
	}
	private void UpdateMeshVertices(float[,] heightMap) {
		int heightMeshDepth = heightMap.GetLength (0);
		int heightMeshWidth = heightMap.GetLength (1);
		
		Mesh mesh = meshFilter.mesh;
		Vector3[] meshVertices = mesh.vertices;

		// iterate through all the heightMap coordinates, updating the vertex index
		int vertexIndex = 0;
		for (int zIndex = 0; zIndex < heightMeshDepth; zIndex++) {
			for (int xIndex = 0; xIndex < heightMeshWidth; xIndex++) {
				float height = heightMap [zIndex, xIndex];

				Vector3 vertex = meshVertices [vertexIndex];
				// change the vertex Y coordinate, proportional to the height value. The height value is evaluated by the heightCurve function, in order to correct it.
				meshVertices[vertexIndex] = new Vector3(vertex.x, heightCurve.Evaluate(height) * heightMultiplier, vertex.z);
				vertexIndex++;
			}
		}

		// update the vertices in the mesh and update its properties
		mesh.vertices = meshVertices;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		// update the mesh collider
		meshCollider.sharedMesh = mesh;
	}
	
	 private void ParityScatter(Prefab prefab, float[,] matrix, float value) {
		 if (value > 100)
		 {
			 // Ex. input is 175 = 25% chance of spawn per tile
			 int chance = (int)value - 100;
			 if (Random.Range(0, 100) < chance)
			 {
				 return;
			 }
		 }
		 
		 int r = (int)(value / 100f * Math.Sqrt(tileWidth * tileDepth)); // radius of comparison (from percentage/100 to ratio of radius/tilesize)
		 
		 // Debug.Log($"Raw input: {value}");
		 // Debug.Log($"Tile Size: {Math.Sqrt(tileWidth * tileDepth)}");
		 // Debug.Log($"Parity Scatter Radius: { r }/{  Math.Sqrt(tileWidth * tileDepth) }");

		 // For every value in 2d arr
		for (int i = 0; i < tileWidth; i++) {
			for (int j = 0; j < tileDepth; j++) {
			    float max = matrix[i, j]; // Start with our current value
			    
			    // For every value in 2d arr, check neighbors up to r away
				for (int ii = i - r; ii <= i + r; ii++) {
	 				for (int jj = j - r; jj <= j + r; jj++) {
	 					if (ii >= 0 && jj >= 0 && ii < tileWidth && jj < tileDepth) {
			                // If it is bigger, save its location to compare to our own
	 						float val = matrix[ii, jj];
	 						if (val > max) {
	 							max = val;
	 						}
	 					}
	 				}
				}
				
			    // Max is now the biggest number in our radius. If its equal to our value, we are the biggest. Instantiate.
			    if(Mathf.Approximately(max, matrix[i, j])){ 
				    InstantiateAtTilePosition(prefab, i, j);
			    }
			}
		}
	 }

	 private void NonParityScatter(Prefab prefab, float density)
	 {
		 int n; // decimal density decider (for values less than 1/tile)
		 if (density < 1f)
		 {
			 float chance = Random.Range(0f, 1f);
			 if (chance > density)
			 {
				 return;
			 }
			 n = 1;
		 }
		 else
		 {
			 n = (int)(density/100f * tileWidth * tileDepth);
		 }
		 
		 // Random Per Tile
		 // convert into decimal, then multiply by total slots (width*length)
		 List<KeyValuePair<float,float>> distribution = new();
		 while (distribution.Count < n) {
			 float a = Random.Range(0f, tileWidth);
			 float b = Random.Range(0f, tileDepth);
			 KeyValuePair<float, float> pair = new (a, b);
			 if (!distribution.Contains(pair))
			 {
				 distribution.Add(pair);
				 InstantiateAtTilePosition(prefab, a, b);
			 }
		 }
	 }

	 private void InstantiateAtTilePosition(Prefab prefab, float x, float y)
	 {
		 Vector3 pos = transform.position;
		 Vector3 prefabPos = new(pos.x + x - tileWidth/2f + 0.5f, pos.y + 10, pos.z + y - tileDepth/2f + 0.5f);
		 Ray ray = new(prefabPos, Vector3.down);
		 if (Physics.Raycast(ray, out var hit, 15f))
		 {
			 //Debug.DrawRay(prefabPos, hit.normal * 4, Color.red, 55f);

			 // unless prefab is an enemy, prefabs must spawn atop empty ground
			 if (hit.collider.gameObject.CompareTag("Floor") || prefab.prefab.CompareTag("Enemy"))
			 {
				 if (prefab.prefab.CompareTag("Rock") && Vector3.Distance(hit.point, new Vector3(mapWidth/2*tileWidth, 0.5f, mapDepth/2*mapDepth)) < 20f)
				 {
					 return; // Reduce chance of rocks clipping cabin prefab (band aid)
				 }
				 
				 // Place our object, rotate it to face correctly, and then scale it according to a bell curve
				 GameObject placedPrefab = Instantiate(prefab.prefab, hit.point, Quaternion.LookRotation(hit.normal), transform);
				 placedPrefab.transform.Rotate(0, 0, Random.Range(0f,360f), Space.Self);
				 placedPrefab.transform.Rotate(90, 0, 0, Space.Self);
			 
				 float scaleFactor = 1;
				 switch (prefab.prefab.tag)
				 {
					 case "Enemy":
						 placedPrefab.transform.Translate(0,5f,0,Space.World);
						 break;
					 case "Stump":
						 scaleFactor = 3f;
						 break;
					 case "ColorShroom":
						 scaleFactor = 17f;
						 placedPrefab.transform.Translate(0,-1.5f,0,Space.Self);
						 break;
					 case "BigShroom":
						 scaleFactor = 8f;
						 placedPrefab.transform.Translate(0,-1.5f,0,Space.Self);
						 break;
					 case "Bush":
						 scaleFactor = 4f;
						 break;
					 case "Tree" :
						 break;
					 case "Rock" :
						 break;
				 }
				 
				 if (prefab.useScaleCurve)
				 {
					placedPrefab.transform.localScale = Vector3.one * scaleFactor * (prefab.scale.Evaluate(Random.Range(0f, 1f)) + 0.5f);
				 }
				 else
				 {
					 placedPrefab.transform.localScale = Vector3.one * scaleFactor;
				 }
			  }
		 }
	 }

	 private void BuildNavMesh(GameObject o)
	 {
		 // Get or add the NavMeshSurface component to the game object
		 NavMeshSurface navMeshSurface = o.GetComponent<NavMeshSurface>();
		 if (navMeshSurface != null)
		 {
			 navMeshSurface.BuildNavMesh();
		 }
	 }
}

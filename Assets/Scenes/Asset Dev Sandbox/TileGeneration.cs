using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
	
	public GameObject treePrefab, spherePrefab;
	
	[SerializeField] private MeshRenderer tileRenderer;
	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private MeshCollider meshCollider;

	private float tileWidth, tileDepth, scatterDensity, scatterRadius;
	
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
		
		//yield return new WaitForSeconds(0.1f);
		yield return 0;
		
		
		
		float[,] scatterMap = GenerateScatterMap(offsetX, offsetZ);
		
		nonParityScatter(scatterMap, scatterDensity);
		
		GenerateTrees(scatterMap, scatterRadius);
		
		yield return 0;
		
		// build a Texture2D from the height map
		Texture2D tileTexture = BuildTexture (heightMap);
		tileRenderer.material.mainTexture = tileTexture;
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
		
		float texturescaleFactor = (float)texture1.width / textureWidth;
		float heightScaleFactor = (float)textureWidth / heightMapWidth;
		
		float[,] hiResHeight = UpsampleFloat2DArray(heightMap, heightScaleFactor);

		Debug.Log($"Original Texture Resolution: {texture1.width}");
		Debug.Log($"Original Heightmap Resolution: {heightMapWidth}");
		Debug.Log($"Selected Texture Resolution: {textureWidth}");
		Debug.Log($"Height Map -> Texture Scale Factor: {heightScaleFactor}");
		Debug.Log($"Texture Down-sampling Factor: {texturescaleFactor}");

		Color[] colorMap = new Color[textureWidth * textureWidth];

		for (int zIndex = 0; zIndex < textureWidth; zIndex++) {
			for (int xIndex = 0; xIndex < textureWidth; xIndex++) {
				// transform the 2D map index is an Array index
				int colorIndex = zIndex * textureWidth + xIndex;

				float heightWeight = blendCurve.Evaluate(hiResHeight[zIndex, xIndex]);
				Vector2Int scaledUV = new((int)(zIndex * texturescaleFactor), (int)(xIndex * texturescaleFactor));
				
				colorMap[colorIndex] =  Color.Lerp(texture2.GetPixel(scaledUV.x, scaledUV.y), texture1.GetPixel(scaledUV.x, scaledUV.y), heightWeight);
			}
		}

		// create a new texture and set its pixel colors
		Texture2D tileTexture = new Texture2D (textureWidth, textureWidth);
		tileTexture.wrapMode = TextureWrapMode.Repeat;
		tileTexture.SetPixels (colorMap);
		tileTexture.Apply();

		return tileTexture;
	}
	
	private static float[,] UpsampleFloat2DArray(float[,] inputArray, float scaleFactor)
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
		int tileDepth = heightMap.GetLength (0);
		int tileWidth = heightMap.GetLength (1);

		Mesh mesh = meshFilter.mesh;
		Vector3[] meshVertices = mesh.vertices;

		// iterate through all the heightMap coordinates, updating the vertex index
		int vertexIndex = 0;
		for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
			for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
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
	
	 private void GenerateTrees(float[,] matrix, float density) {
		int scatterLength = matrix.GetLength(0);
		int scatterWidth = matrix.GetLength(1);
		int r = (int)(density / 100f) * scatterLength;
		//int r = (int)density;
		 r = 11;
		Debug.Log($"Density: {r}");

		// For every value in 2d arr
		for (int i = 0; i < scatterLength; i++) {
			for (int j = 0; j < scatterWidth; j++) {
			    float max = matrix[i, j];
			    
			    // For every value in 2d arr, check neighbors up to r away
				for (int ii = i - r; ii <= i + r; ii++) {
	 				for (int jj = j - r; jj <= j + r; jj++) {
	 					if (ii >= 0 && jj >= 0 && ii < scatterLength && jj < scatterWidth) {
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
					Vector3 treePos = new(transform.position.x + i - scatterWidth/2f + 0.5f, transform.position.y + 10, transform.position.z + j - scatterWidth/2f + 0.5f);

					Ray ray = new(treePos, Vector3.down);
					if (Physics.Raycast(ray, out var hit, 15f))
	 				{
	 					treePos = hit.point;
	 				}
	 				Instantiate(treePrefab, treePos, Quaternion.identity);
				}
			}
		}
	 }

	 private void nonParityScatter(float[,] matrix, float density)
	 {
		 int scatterLength = matrix.GetLength(0);
		 int scatterWidth = matrix.GetLength(1);
		 int n = 0;
		 
		 
		 // decimal density decider
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
			 n = (int)(density/100f * scatterLength * scatterWidth);
		 }
		 
		 
		 // Random Per Tile
		 // convert into decimal, then multiply by total slots (width*length)

		 List<KeyValuePair<int,int>> distribution = new List<KeyValuePair<int,int>>();
		
		 while (distribution.Count < n) {
			 int a = Random.Range(0, scatterLength);
			 int b = Random.Range(0, scatterWidth);
			 KeyValuePair<int, int> pair = new (a, b);
			 if (!distribution.Contains(pair))
			 {
				 distribution.Add(pair);
				 Vector3 spherePos = new(transform.position.x + a - scatterLength/2f + 0.5f, transform.position.y + 10, transform.position.z + b - scatterWidth/2f + 0.5f);
				 Ray ray2 = new(spherePos, Vector3.down);
				 if (Physics.Raycast(ray2, out var hit2, 15f))
				 {
					 spherePos = hit2.point;
				 }
				 Instantiate(spherePrefab, spherePos, Quaternion.identity);
			 }
		 }
	 }
}

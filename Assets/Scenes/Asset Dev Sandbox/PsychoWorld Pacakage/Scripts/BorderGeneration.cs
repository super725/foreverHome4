using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderGeneration : MonoBehaviour
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

	private Prefab[] prefabs;

	[SerializeField] private MeshRenderer tileRenderer;
	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private MeshCollider meshCollider;

	private float tileWidth, tileDepth;
	private int mapWidth, mapDepth, borderThickness;

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

		mapDepth = args.mapDepth;
		mapWidth = args.mapWidth;
		borderThickness = args.borderThickness;
		
		Vector3 tileSize = GetComponent<MeshRenderer>().bounds.size;
		tileWidth = (int)tileSize.x;
		tileDepth = (int)tileSize.z;
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
		
		//build a Texture2D from the height map
		Texture2D tileTexture = BuildTexture (heightMap);
		tileRenderer.material.mainTexture = tileTexture;
		tileRenderer.material.SetFloat(Metallic,0.15f);
		tileRenderer.material.SetFloat(Glossiness,0.15f);
	}

	private float[,] GenerateHeightMap(float offsetX, float offsetZ) {
		// Use our meshFilter vertices to figure out how the dimensions of our heightmap
		Vector3[] meshVertices = meshFilter.mesh.vertices;
		int tileMeshDepth = (int)Mathf.Sqrt (meshVertices.Length);
		int tileMeshWidth = tileMeshDepth;
		
		return noiseMapGeneration.GenerateNoiseMap (tileMeshDepth, tileMeshWidth, levelScale, offsetX, offsetZ, heightWave);
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
		
		
		// This is a bit different than our normal map height mesh adjustment: We need to taper the edges via blending to 0.
		// First figure out where we are in the overall map. 
		// This can be done with evaluating our x and z coords. at least one will be 0 or max. that is our edge.
		Vector3 pos = transform.position;
		
		// West
		if(pos.x == (borderThickness-1) * tileWidth && pos.z >= borderThickness * tileDepth && pos.z <= (mapDepth-borderThickness-1) * tileDepth)
		{
			for (int r = 0; r < heightMeshDepth; r++)
			{
				float height = heightMap[r, 0];
				Vector3 vertex = meshVertices[r * heightMeshWidth];
				meshVertices[r * heightMeshWidth] = new Vector3(tileWidth/2, heightCurve.Evaluate(height) * heightMultiplier, vertex.z);

				// Now that we have the sample, scale down
				float scaleHeight = heightCurve.Evaluate(height) * heightMultiplier;
				for (int i = 1; i < heightMeshWidth; i++)
				{
					meshVertices[i + r * heightMeshWidth].y = scaleHeight - (i * scaleHeight) / (heightMeshWidth - 1) ;
				}
			}
		}
		//East
		if (pos.x == (mapWidth-borderThickness) * tileWidth && pos.z >= borderThickness * tileDepth && pos.z <= (mapDepth-borderThickness-1) * tileDepth)
		{
			for (int r = 0; r < heightMeshDepth; r++)
			{
				float height = heightMap[r , heightMeshWidth - 1];
				Vector3 vertex = meshVertices[r * heightMeshWidth + (heightMeshWidth - 1)];
				meshVertices[r * heightMeshWidth + (heightMeshWidth - 1)] = new Vector3(-tileWidth/2, heightCurve.Evaluate(height) * heightMultiplier, vertex.z);
		
				float scaleHeight = heightCurve.Evaluate(height) * heightMultiplier;
				for (int i = 0; i < heightMeshWidth-1; i++)
				{
					meshVertices[i + r * heightMeshWidth].y = (i * scaleHeight) / (heightMeshWidth - 1) ;
				}
			}
		}
		//South
		if (pos.z == (borderThickness-1) * tileDepth && pos.x >= borderThickness * tileWidth && pos.x <= (mapWidth-borderThickness-1) * tileWidth)
		{
			for (int x = 0; x < heightMeshWidth; x++)
			{
				float height = heightMap[0, x];
				Vector3 vertex = meshVertices[x];
				meshVertices[x] = new Vector3(vertex.x, heightCurve.Evaluate(height) * heightMultiplier, tileDepth/2);
		
				float scaleHeight = heightCurve.Evaluate(height) * heightMultiplier;
				for (int i = 1; i < heightMeshDepth; i++)
				{
					meshVertices[x + i * heightMeshDepth].y = scaleHeight - (i * scaleHeight) / (heightMeshDepth - 1);
				}
				
			}
		}
		//North
		if (pos.z == (mapDepth-borderThickness) * tileDepth && pos.x >= borderThickness * tileWidth && pos.x <= (mapWidth-borderThickness-1) * tileWidth)
		{
			for (int x = 0; x < heightMeshWidth; x++)
			{
				float height = heightMap[heightMeshDepth - 1, x];
				Vector3 vertex = meshVertices[x + (heightMeshDepth - 1) * heightMeshWidth];
				meshVertices[x + (heightMeshDepth - 1) * heightMeshWidth] = new Vector3(vertex.x, heightCurve.Evaluate(height) * heightMultiplier, -tileDepth/2);
		
				float scaleHeight = heightCurve.Evaluate(height) * heightMultiplier;
				for (int i = 0; i < heightMeshDepth - 1; i++)
				{
					meshVertices[x + (i * heightMeshDepth)].y = (i * scaleHeight) / (heightMeshDepth - 1);
				}
			}
		}
		//
		// // CORNER 0.0
		if (pos.x == (borderThickness-1) * tileWidth && pos.z == (borderThickness-1) * tileDepth)
		{
			float scaleHeight = heightCurve.Evaluate(heightMap[0, 0]) * heightMultiplier;
			for (int row = 0; row < heightMeshWidth; row++)
			{
				for (int col = 0; col < heightMeshDepth; col++)
				{
					float y;
					if (row == 0 && col == 0)
					{
						y = scaleHeight;
					}
					else
					{
						float distanceFromCorner = Mathf.Sqrt(row * row + col * col);
						y = scaleHeight * (1 - distanceFromCorner / (heightMeshWidth - 1));
						y = Mathf.Max(0, y);
					}
					meshVertices[row + col * heightMeshWidth].y = y;
				}
			}
		}
		
		if (pos.x == (mapWidth-borderThickness) * tileWidth && pos.z == (borderThickness-1) * tileDepth)
		{
			float scaleHeight = heightCurve.Evaluate(heightMap[0, heightMeshWidth - 1]) * heightMultiplier;
			for (int row = 0; row < heightMeshWidth; row++)
			{
				for (int col = 0; col < heightMeshDepth; col++)
				{
					float y;
					if (row == heightMeshWidth - 1 && col == 0)
					{
						y = scaleHeight;
					}
					else
					{
						float distanceFromCorner = Mathf.Sqrt((row - heightMeshWidth + 1) * (row - heightMeshWidth + 1) + col * col);
						y = scaleHeight * (1 - distanceFromCorner / (heightMeshWidth - 1));
						y = Mathf.Max(0, y);
					}
					meshVertices[row + col * heightMeshWidth].y = y;
				}
			}
		}
		
		if (pos.x == (borderThickness-1) * tileWidth && pos.z == (mapDepth-borderThickness) * tileDepth)
		{
			float scaleHeight = heightCurve.Evaluate(heightMap[heightMeshDepth - 1, 0]) * heightMultiplier;
			for (int row = 0; row < heightMeshWidth; row++)
			{
				for (int col = 0; col < heightMeshDepth; col++)
				{
					float y;
					if (row == 0 && col == heightMeshDepth - 1)
					{
						y = scaleHeight;
					}
					else
					{
						float distanceFromCorner = Mathf.Sqrt(row * row + (col - heightMeshDepth + 1) * (col - heightMeshDepth + 1));
						y = scaleHeight * (1 - distanceFromCorner / (heightMeshWidth - 1));
						y = Mathf.Max(0, y);
					}
					meshVertices[row + col * heightMeshWidth].y = y;
				}
			}
		}

		if (pos.x == (mapWidth-borderThickness) * tileWidth && pos.z == (mapDepth-borderThickness) * tileDepth)
		{
			float scaleHeight = heightCurve.Evaluate(heightMap[heightMeshDepth - 1, heightMeshWidth - 1]) * heightMultiplier;
			for (int row = 0; row < heightMeshWidth; row++)
			{
				for (int col = 0; col < heightMeshDepth; col++)
				{
					float y;
					if (row == heightMeshWidth - 1 && col == heightMeshDepth - 1)
					{
						y = scaleHeight;
					}
					else
					{
						float distanceFromCorner = Mathf.Sqrt((row - heightMeshWidth + 1) * (row - heightMeshWidth + 1) + (col - heightMeshDepth + 1) * (col - heightMeshDepth + 1));
						y = scaleHeight * (1 - distanceFromCorner / (heightMeshWidth - 1));
						y = Mathf.Max(0, y);
					}
					meshVertices[row + col * heightMeshWidth].y = y;
				}
			}
		}

		// update the vertices in the mesh and update its properties
		mesh.vertices = meshVertices;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		// update the mesh collider
		meshCollider.sharedMesh = mesh;
	}
}

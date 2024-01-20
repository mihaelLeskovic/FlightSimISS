using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    public void Setup(int tileCount, float scale, int resolution, int noiseIntensity, float noiseScale)
    {
        this.tileCount = tileCount;
        this.scale = scale;
        this.resolution = resolution;
        this.noiseIntensity = noiseIntensity;
        this.noiseScale = noiseScale;
    }

    [Serializable]
    public class TerrainType
    {
        public float minHeight;
        public float maxHeight;
        public Texture2D texture;
    }

    [SerializeField] int tileCount;
    [SerializeField] float scale;
    [SerializeField] int resolution;
    [SerializeField] int noiseIntensity;
    [SerializeField] float noiseScale;

    [SerializeField] TerrainType[] terrainTypes;

    GameObject[,] terrainCache;
    Mesh[,] meshCache;
    MeshCollider[,] meshColliders;
    float[,] noiseMap;

    private void Start()
    {
        terrainCache = new GameObject[tileCount, tileCount];
        meshCache = new Mesh[tileCount, tileCount];
        meshColliders = new MeshCollider[tileCount, tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            for (int j = 0; j < tileCount; j++)
            {
                terrainCache[i, j] = new GameObject("TerrainTile");
                terrainCache[i, j].transform.position = new Vector3(i * scale, 0, j * scale);
                terrainCache[i, j].transform.parent = transform;
                terrainCache[i, j].tag = "terrain";
                var meshFilter = terrainCache[i, j].AddComponent<MeshFilter>();
                meshColliders[i, j] = terrainCache[i, j].AddComponent<MeshCollider>();
                var meshRenderer = terrainCache[i, j].AddComponent<MeshRenderer>();
                meshFilter.mesh = GenerateMesh(scale, resolution);
                meshCache[i, j] = meshFilter.mesh;
                meshRenderer.material.shader = Shader.Find("Shader Graphs/TerrainShader");
                //TODO setFloat TexTiling to shader
            }
        }

        Vector3[] vertices = terrainCache[0, 0].GetComponent<MeshFilter>().mesh.vertices;

        int depth = (int)Mathf.Sqrt(vertices.Length) * tileCount;
        int tileDepth = (int)Mathf.Sqrt(vertices.Length);
        int tileWidth = (int)Mathf.Sqrt(vertices.Length);
        int width = depth;

        noiseMap = GenerateNoiseMap(depth, width, noiseScale);

        for (int i = 1; i < tileCount; i++)
        {
            for (int j = 0; j < noiseMap.GetLength(1); j++)
            {
                noiseMap[i * tileDepth, j] = noiseMap[i * tileDepth - 1, j];
            }
        }

        for (int i = 1; i < tileCount; i++)
        {
            for (int j = 0; j < noiseMap.GetLength(0); j++)
            {
                noiseMap[j, i * tileWidth] = noiseMap[j, i * tileWidth - 1];
            }
        }

        for (int i = 0; i < tileCount; i++)
        {
            for (int j = 0; j < tileCount; j++)
            {
                GenerateTile(terrainCache[i, j], tileWidth * j, tileDepth * i, tileWidth, tileDepth);
            }
        }

        for (int i = 0; i < tileCount; i++)
        {
            for (int j = 0; j < tileCount; j++)
            {
                meshColliders[i, j].sharedMesh = meshCache[i, j];
            }
        }

        transform.localPosition = new Vector3(-tileCount * scale / 2, 0, -tileCount * scale / 2);
    }


    Mesh GenerateMesh(float scale, int resolution)
    {
        Mesh planeMesh = new Mesh();

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        float step = scale / resolution;

        for (int i = 0; i < resolution + 1; i++)
        {
            for( int j = 0; j < resolution + 1; j++)
            {
                uv.Add(new Vector2((float)j / resolution, (float)i / resolution)); 
                vertices.Add(new Vector3(j * step, 0, i * step));
            }
        }

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                int pos = (i * resolution) + i + j;

                triangles.Add(pos);
                triangles.Add(pos + resolution + 1);
                triangles.Add(pos + resolution + 2);

                triangles.Add(pos);
                triangles.Add(pos + resolution + 2);
                triangles.Add(pos + 1);
            }
        }

        planeMesh.vertices = vertices.ToArray();
        planeMesh.triangles = triangles.ToArray();
        planeMesh.uv = uv.ToArray();

        return planeMesh;
    }

    void GenerateTile(GameObject target, int offsetByTileX, int offsetByTileY, int tileWidth, int tileDepth)
    {
        var meshFilter = target.GetComponent<MeshFilter>();
        Vector3[] vertices = meshFilter.mesh.vertices;

        var subHeightMap = new float[tileWidth, tileDepth];
        for (int i = offsetByTileX, x = 0; i < offsetByTileX + tileDepth; i++, x++)
        {
            for (int j = offsetByTileY, y = 0; j < offsetByTileY + tileWidth; j++, y++)
            {
                subHeightMap[x, y] = noiseMap[i, j];
            }
        }

        int vertexIndex = 0;
        int hmDepth = subHeightMap.GetLength(0);
        int hmWidth = subHeightMap.GetLength(1);

        for (int i = 0; i < hmDepth; i++)
        {
            for (int j = 0; j < hmWidth; j++)
            {
                float height = subHeightMap[i, j];

                Vector3 vertex = vertices[vertexIndex];
                vertices[vertexIndex] = new Vector3(vertex.x, height * noiseIntensity, vertex.z);

                vertexIndex++;
            }
        }

        meshFilter.mesh.vertices = vertices;

        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateTangents();

        //Texture2D texture = BuildTexture(heightMap);
        //meshRenderer.material.mainTexture = texture;
        //meshRenderer.material.SetTextureScale("_BaseColorMap", new Vector2(50f, 50f));
        //meshRenderer.material.SetFloat("_Metallic", 0f);
        //meshRenderer.material.SetFloat("_Smoothness", 0f);
    }

    Texture2D BuildTexture(float[,] heightMap)
    {
        int depth = heightMap.GetLength(0); 
        int width = heightMap.GetLength(1);

        Color[] colorMap = new Color[depth * width];
        for (int i = 0;  i < depth; i++)
        {
            for (int j = 0; j < width; j++)
            {
                colorMap[i * width + j] = Color.Lerp(Color.black, Color.white, heightMap[i, j]);
            }
        }

        //TODO give texture based on height
        Texture2D texture = new Texture2D(width, depth);
        texture.wrapMode = TextureWrapMode.Clamp;
        //texture.SetPixels(colorMap);
        texture = terrainTypes[0].texture;
        texture.Apply();

        return texture;
    }

    float[,] GenerateNoiseMap(int depth, int width, float scale)
    {
        float[,] noiseMap = new float[depth, width];

        for (int i = 0; i < depth; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float sampleX = j / scale;
                float sampleZ = i / scale;
                float noise = Mathf.PerlinNoise(sampleX, sampleZ);

                noiseMap[i, j] = noise;
            }
        }

        return noiseMap;
    }
}

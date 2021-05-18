using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelWorld : MonoBehaviour
{
    [SerializeField] string worldDataTextureName;
    [SerializeField] Vector3Int worldSize;
    [SerializeField] string skyboxTextureName;
    [SerializeField] Texture2D skybox;

    private ComputeShaderRenderer shaderRenderer;
    private ComputeShader shader;
    private int worldDataTextureID;
    private int skyboxTextureID;

    Array3D<byte> worldData;
    private Texture3D worldDataTexture;

    private void Start()
    {
        SetupReferences();
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        for (int i = 0; i < worldData.SizeX; ++i)
        {
            for (int j = 0; j < worldData.SizeY; ++j)
            {
                for (int k = 0; k < worldData.SizeZ; ++k)
                {
                    if (UnityEngine.Random.value < 0.1)
                    {
                        worldData[i, j, k] = 1;
                    }
                }
            }
        }

        worldDataTexture.SetPixelData(worldData.AsOneDimensionalArray(), 0);
        worldDataTexture.Apply();
    }

    private void SetupReferences()
    {
        shaderRenderer = GetComponent<ComputeShaderRenderer>();
        shader = shaderRenderer.RenderingShader;

        worldData = new Array3D<byte>(worldSize.x, worldSize.y, worldSize.z);
        worldDataTexture = new UnityEngine.Texture3D(worldSize.x, worldSize.y, worldSize.z, TextureFormat.R8, false);
        worldDataTexture.filterMode = FilterMode.Point;

        worldDataTextureID = Shader.PropertyToID(worldDataTextureName);
        skyboxTextureID = Shader.PropertyToID(skyboxTextureName);

        shader.SetTexture(shaderRenderer.MainKernelIndex, worldDataTextureID, worldDataTexture);
        shader.SetTexture(shaderRenderer.MainKernelIndex, skyboxTextureID, skybox);

        shaderRenderer.OnRender += OnRender;
    }

    void OnRender()
    {
        SetShaderParameters();
    }

    private void SetShaderParameters()
    {
        Camera camera = Camera.current;
        shader.SetMatrix("cameraToWorld", camera.cameraToWorldMatrix);
        shader.SetMatrix("cameraInverseProjection", camera.projectionMatrix.inverse);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderRenderer : MonoBehaviour
{
    public ComputeShader RenderingShader => renderingShader;
    public int MainKernelIndex { get; private set; }
    public Action OnRender;

    [SerializeField] private ComputeShader renderingShader;
    [SerializeField] private string mainKernelName;
    [SerializeField] string targetName;
    [SerializeField] string addShaderName;
    [SerializeField] string currentSampleName;
    [SerializeField] string pixelOffsetName;

    private RenderTexture target;
    private int targetID;
    private uint localWorkGroupSizeX;
    private uint localWorkGroupSizeY;
    private uint currentSample;
    private Material addMaterial;
    private int currentSampleID;
    private int pixelOffsetID;

    private void Start()
    {
        MainKernelIndex = renderingShader.FindKernel(mainKernelName);
        targetID = Shader.PropertyToID(targetName);
        addMaterial = new Material(Shader.Find(addShaderName));
        currentSampleID = Shader.PropertyToID(currentSampleName);
        pixelOffsetID = Shader.PropertyToID(pixelOffsetName);

        renderingShader.GetKernelThreadGroupSizes(MainKernelIndex, out localWorkGroupSizeX, out localWorkGroupSizeY, out _);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        UpdateSampleCount();
        OnRender();
        InitRenderTexture();
        Render(destination);
    }

    private void UpdateSampleCount()
    {
        if (transform.hasChanged)
        {
            currentSample = 0;
            transform.hasChanged = false;
        }
        else
        {
            ++currentSample;
        }
    }

    private void Render(RenderTexture destination)
    {
        RenderingShader.SetVector(pixelOffsetID, new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
        
        int threadGroupsX = Mathf.CeilToInt(Screen.width / localWorkGroupSizeX);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / localWorkGroupSizeY);
        renderingShader.Dispatch(MainKernelIndex, threadGroupsX, threadGroupsY, 1);

        addMaterial.SetFloat(currentSampleID, currentSample);
        Graphics.Blit(target, destination, addMaterial);
    }

    private void InitRenderTexture()
    {
        if (target == null || target.width != Screen.width || target.height != Screen.height)
        {
            if (target != null)
                target.Release();

            target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create();

            renderingShader.SetTexture(MainKernelIndex, targetID, target);
        }
    }
}
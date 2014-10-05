// --------------------------------------------------------------------------------------------------------------------
// <copyright file="D3D9.cs" company="">
//   
// </copyright>
// <summary>
//   The full list of IDirect3DDevice9 functions with the correct index
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Hook
{
    /// <summary>
    /// The full list of IDirect3DDevice9 functions with the correct index
    /// </summary>
    public enum Direct3DDevice9FunctionOrdinals : short
    {
        /// <summary>
        /// The query interface.
        /// </summary>
        QueryInterface = 0, 

        /// <summary>
        /// The add ref.
        /// </summary>
        AddRef = 1, 

        /// <summary>
        /// The release.
        /// </summary>
        Release = 2, 

        /// <summary>
        /// The test cooperative level.
        /// </summary>
        TestCooperativeLevel = 3, 

        /// <summary>
        /// The get available texture mem.
        /// </summary>
        GetAvailableTextureMem = 4, 

        /// <summary>
        /// The evict managed resources.
        /// </summary>
        EvictManagedResources = 5, 

        /// <summary>
        /// The get direct 3 d.
        /// </summary>
        GetDirect3D = 6, 

        /// <summary>
        /// The get device caps.
        /// </summary>
        GetDeviceCaps = 7, 

        /// <summary>
        /// The get display mode.
        /// </summary>
        GetDisplayMode = 8, 

        /// <summary>
        /// The get creation parameters.
        /// </summary>
        GetCreationParameters = 9, 

        /// <summary>
        /// The set cursor properties.
        /// </summary>
        SetCursorProperties = 10, 

        /// <summary>
        /// The set cursor position.
        /// </summary>
        SetCursorPosition = 11, 

        /// <summary>
        /// The show cursor.
        /// </summary>
        ShowCursor = 12, 

        /// <summary>
        /// The create additional swap chain.
        /// </summary>
        CreateAdditionalSwapChain = 13, 

        /// <summary>
        /// The get swap chain.
        /// </summary>
        GetSwapChain = 14, 

        /// <summary>
        /// The get number of swap chains.
        /// </summary>
        GetNumberOfSwapChains = 15, 

        /// <summary>
        /// The reset.
        /// </summary>
        Reset = 16, 

        /// <summary>
        /// The present.
        /// </summary>
        Present = 17, 

        /// <summary>
        /// The get back buffer.
        /// </summary>
        GetBackBuffer = 18, 

        /// <summary>
        /// The get raster status.
        /// </summary>
        GetRasterStatus = 19, 

        /// <summary>
        /// The set dialog box mode.
        /// </summary>
        SetDialogBoxMode = 20, 

        /// <summary>
        /// The set gamma ramp.
        /// </summary>
        SetGammaRamp = 21, 

        /// <summary>
        /// The get gamma ramp.
        /// </summary>
        GetGammaRamp = 22, 

        /// <summary>
        /// The create texture.
        /// </summary>
        CreateTexture = 23, 

        /// <summary>
        /// The create volume texture.
        /// </summary>
        CreateVolumeTexture = 24, 

        /// <summary>
        /// The create cube texture.
        /// </summary>
        CreateCubeTexture = 25, 

        /// <summary>
        /// The create vertex buffer.
        /// </summary>
        CreateVertexBuffer = 26, 

        /// <summary>
        /// The create index buffer.
        /// </summary>
        CreateIndexBuffer = 27, 

        /// <summary>
        /// The create render target.
        /// </summary>
        CreateRenderTarget = 28, 

        /// <summary>
        /// The create depth stencil surface.
        /// </summary>
        CreateDepthStencilSurface = 29, 

        /// <summary>
        /// The update surface.
        /// </summary>
        UpdateSurface = 30, 

        /// <summary>
        /// The update texture.
        /// </summary>
        UpdateTexture = 31, 

        /// <summary>
        /// The get render target data.
        /// </summary>
        GetRenderTargetData = 32, 

        /// <summary>
        /// The get front buffer data.
        /// </summary>
        GetFrontBufferData = 33, 

        /// <summary>
        /// The stretch rect.
        /// </summary>
        StretchRect = 34, 

        /// <summary>
        /// The color fill.
        /// </summary>
        ColorFill = 35, 

        /// <summary>
        /// The create offscreen plain surface.
        /// </summary>
        CreateOffscreenPlainSurface = 36, 

        /// <summary>
        /// The set render target.
        /// </summary>
        SetRenderTarget = 37, 

        /// <summary>
        /// The get render target.
        /// </summary>
        GetRenderTarget = 38, 

        /// <summary>
        /// The set depth stencil surface.
        /// </summary>
        SetDepthStencilSurface = 39, 

        /// <summary>
        /// The get depth stencil surface.
        /// </summary>
        GetDepthStencilSurface = 40, 

        /// <summary>
        /// The begin scene.
        /// </summary>
        BeginScene = 41, 

        /// <summary>
        /// The end scene.
        /// </summary>
        EndScene = 42, 

        /// <summary>
        /// The clear.
        /// </summary>
        Clear = 43, 

        /// <summary>
        /// The set transform.
        /// </summary>
        SetTransform = 44, 

        /// <summary>
        /// The get transform.
        /// </summary>
        GetTransform = 45, 

        /// <summary>
        /// The multiply transform.
        /// </summary>
        MultiplyTransform = 46, 

        /// <summary>
        /// The set viewport.
        /// </summary>
        SetViewport = 47, 

        /// <summary>
        /// The get viewport.
        /// </summary>
        GetViewport = 48, 

        /// <summary>
        /// The set material.
        /// </summary>
        SetMaterial = 49, 

        /// <summary>
        /// The get material.
        /// </summary>
        GetMaterial = 50, 

        /// <summary>
        /// The set light.
        /// </summary>
        SetLight = 51, 

        /// <summary>
        /// The get light.
        /// </summary>
        GetLight = 52, 

        /// <summary>
        /// The light enable.
        /// </summary>
        LightEnable = 53, 

        /// <summary>
        /// The get light enable.
        /// </summary>
        GetLightEnable = 54, 

        /// <summary>
        /// The set clip plane.
        /// </summary>
        SetClipPlane = 55, 

        /// <summary>
        /// The get clip plane.
        /// </summary>
        GetClipPlane = 56, 

        /// <summary>
        /// The set render state.
        /// </summary>
        SetRenderState = 57, 

        /// <summary>
        /// The get render state.
        /// </summary>
        GetRenderState = 58, 

        /// <summary>
        /// The create state block.
        /// </summary>
        CreateStateBlock = 59, 

        /// <summary>
        /// The begin state block.
        /// </summary>
        BeginStateBlock = 60, 

        /// <summary>
        /// The end state block.
        /// </summary>
        EndStateBlock = 61, 

        /// <summary>
        /// The set clip status.
        /// </summary>
        SetClipStatus = 62, 

        /// <summary>
        /// The get clip status.
        /// </summary>
        GetClipStatus = 63, 

        /// <summary>
        /// The get texture.
        /// </summary>
        GetTexture = 64, 

        /// <summary>
        /// The set texture.
        /// </summary>
        SetTexture = 65, 

        /// <summary>
        /// The get texture stage state.
        /// </summary>
        GetTextureStageState = 66, 

        /// <summary>
        /// The set texture stage state.
        /// </summary>
        SetTextureStageState = 67, 

        /// <summary>
        /// The get sampler state.
        /// </summary>
        GetSamplerState = 68, 

        /// <summary>
        /// The set sampler state.
        /// </summary>
        SetSamplerState = 69, 

        /// <summary>
        /// The validate device.
        /// </summary>
        ValidateDevice = 70, 

        /// <summary>
        /// The set palette entries.
        /// </summary>
        SetPaletteEntries = 71, 

        /// <summary>
        /// The get palette entries.
        /// </summary>
        GetPaletteEntries = 72, 

        /// <summary>
        /// The set current texture palette.
        /// </summary>
        SetCurrentTexturePalette = 73, 

        /// <summary>
        /// The get current texture palette.
        /// </summary>
        GetCurrentTexturePalette = 74, 

        /// <summary>
        /// The set scissor rect.
        /// </summary>
        SetScissorRect = 75, 

        /// <summary>
        /// The get scissor rect.
        /// </summary>
        GetScissorRect = 76, 

        /// <summary>
        /// The set software vertex processing.
        /// </summary>
        SetSoftwareVertexProcessing = 77, 

        /// <summary>
        /// The get software vertex processing.
        /// </summary>
        GetSoftwareVertexProcessing = 78, 

        /// <summary>
        /// The set n patch mode.
        /// </summary>
        SetNPatchMode = 79, 

        /// <summary>
        /// The get n patch mode.
        /// </summary>
        GetNPatchMode = 80, 

        /// <summary>
        /// The draw primitive.
        /// </summary>
        DrawPrimitive = 81, 

        /// <summary>
        /// The draw indexed primitive.
        /// </summary>
        DrawIndexedPrimitive = 82, 

        /// <summary>
        /// The draw primitive up.
        /// </summary>
        DrawPrimitiveUP = 83, 

        /// <summary>
        /// The draw indexed primitive up.
        /// </summary>
        DrawIndexedPrimitiveUP = 84, 

        /// <summary>
        /// The process vertices.
        /// </summary>
        ProcessVertices = 85, 

        /// <summary>
        /// The create vertex declaration.
        /// </summary>
        CreateVertexDeclaration = 86, 

        /// <summary>
        /// The set vertex declaration.
        /// </summary>
        SetVertexDeclaration = 87, 

        /// <summary>
        /// The get vertex declaration.
        /// </summary>
        GetVertexDeclaration = 88, 

        /// <summary>
        /// The set fvf.
        /// </summary>
        SetFVF = 89, 

        /// <summary>
        /// The get fvf.
        /// </summary>
        GetFVF = 90, 

        /// <summary>
        /// The create vertex shader.
        /// </summary>
        CreateVertexShader = 91, 

        /// <summary>
        /// The set vertex shader.
        /// </summary>
        SetVertexShader = 92, 

        /// <summary>
        /// The get vertex shader.
        /// </summary>
        GetVertexShader = 93, 

        /// <summary>
        /// The set vertex shader constant f.
        /// </summary>
        SetVertexShaderConstantF = 94, 

        /// <summary>
        /// The get vertex shader constant f.
        /// </summary>
        GetVertexShaderConstantF = 95, 

        /// <summary>
        /// The set vertex shader constant i.
        /// </summary>
        SetVertexShaderConstantI = 96, 

        /// <summary>
        /// The get vertex shader constant i.
        /// </summary>
        GetVertexShaderConstantI = 97, 

        /// <summary>
        /// The set vertex shader constant b.
        /// </summary>
        SetVertexShaderConstantB = 98, 

        /// <summary>
        /// The get vertex shader constant b.
        /// </summary>
        GetVertexShaderConstantB = 99, 

        /// <summary>
        /// The set stream source.
        /// </summary>
        SetStreamSource = 100, 

        /// <summary>
        /// The get stream source.
        /// </summary>
        GetStreamSource = 101, 

        /// <summary>
        /// The set stream source freq.
        /// </summary>
        SetStreamSourceFreq = 102, 

        /// <summary>
        /// The get stream source freq.
        /// </summary>
        GetStreamSourceFreq = 103, 

        /// <summary>
        /// The set indices.
        /// </summary>
        SetIndices = 104, 

        /// <summary>
        /// The get indices.
        /// </summary>
        GetIndices = 105, 

        /// <summary>
        /// The create pixel shader.
        /// </summary>
        CreatePixelShader = 106, 

        /// <summary>
        /// The set pixel shader.
        /// </summary>
        SetPixelShader = 107, 

        /// <summary>
        /// The get pixel shader.
        /// </summary>
        GetPixelShader = 108, 

        /// <summary>
        /// The set pixel shader constant f.
        /// </summary>
        SetPixelShaderConstantF = 109, 

        /// <summary>
        /// The get pixel shader constant f.
        /// </summary>
        GetPixelShaderConstantF = 110, 

        /// <summary>
        /// The set pixel shader constant i.
        /// </summary>
        SetPixelShaderConstantI = 111, 

        /// <summary>
        /// The get pixel shader constant i.
        /// </summary>
        GetPixelShaderConstantI = 112, 

        /// <summary>
        /// The set pixel shader constant b.
        /// </summary>
        SetPixelShaderConstantB = 113, 

        /// <summary>
        /// The get pixel shader constant b.
        /// </summary>
        GetPixelShaderConstantB = 114, 

        /// <summary>
        /// The draw rect patch.
        /// </summary>
        DrawRectPatch = 115, 

        /// <summary>
        /// The draw tri patch.
        /// </summary>
        DrawTriPatch = 116, 

        /// <summary>
        /// The delete patch.
        /// </summary>
        DeletePatch = 117, 

        /// <summary>
        /// The create query.
        /// </summary>
        CreateQuery = 118, 
    }

    /// <summary>
    /// The direct 3 d device 9 ex function ordinals.
    /// </summary>
    public enum Direct3DDevice9ExFunctionOrdinals : short
    {
        /// <summary>
        /// The set convolution mono kernel.
        /// </summary>
        SetConvolutionMonoKernel = 119, 

        /// <summary>
        /// The compose rects.
        /// </summary>
        ComposeRects = 120, 

        /// <summary>
        /// The present ex.
        /// </summary>
        PresentEx = 121, 

        /// <summary>
        /// The get gpu thread priority.
        /// </summary>
        GetGPUThreadPriority = 122, 

        /// <summary>
        /// The set gpu thread priority.
        /// </summary>
        SetGPUThreadPriority = 123, 

        /// <summary>
        /// The wait for v blank.
        /// </summary>
        WaitForVBlank = 124, 

        /// <summary>
        /// The check resource residency.
        /// </summary>
        CheckResourceResidency = 125, 

        /// <summary>
        /// The set maximum frame latency.
        /// </summary>
        SetMaximumFrameLatency = 126, 

        /// <summary>
        /// The get maximum frame latency.
        /// </summary>
        GetMaximumFrameLatency = 127, 

        /// <summary>
        /// The check device state_.
        /// </summary>
        CheckDeviceState_ = 128, 

        /// <summary>
        /// The create render target ex.
        /// </summary>
        CreateRenderTargetEx = 129, 

        /// <summary>
        /// The create offscreen plain surface ex.
        /// </summary>
        CreateOffscreenPlainSurfaceEx = 130, 

        /// <summary>
        /// The create depth stencil surface ex.
        /// </summary>
        CreateDepthStencilSurfaceEx = 131, 

        /// <summary>
        /// The reset ex.
        /// </summary>
        ResetEx = 132, 

        /// <summary>
        /// The get display mode ex.
        /// </summary>
        GetDisplayModeEx = 133, 
    }

    /// <summary>
    /// The direct 3 d swap chain 9 ordinals.
    /// </summary>
    public enum Direct3DSwapChain9Ordinals : short
    {
        /// <summary>
        /// The query interface.
        /// </summary>
        QueryInterface = 0, 

        /// <summary>
        /// The add ref.
        /// </summary>
        AddRef = 1, 

        /// <summary>
        /// The release.
        /// </summary>
        Release = 2, 

        /// <summary>
        /// The present.
        /// </summary>
        Present = 3, 

        /// <summary>
        /// The get front buffer data.
        /// </summary>
        GetFrontBufferData = 4, 

        /// <summary>
        /// The get back buffer.
        /// </summary>
        GetBackBuffer = 5, 

        /// <summary>
        /// The get raster status.
        /// </summary>
        GetRasterStatus = 6, 

        /// <summary>
        /// The get display mode.
        /// </summary>
        GetDisplayMode = 7, 

        /// <summary>
        /// The get device.
        /// </summary>
        GetDevice = 8, 

        /// <summary>
        /// The get present parameters.
        /// </summary>
        GetPresentParameters = 9
    }
}
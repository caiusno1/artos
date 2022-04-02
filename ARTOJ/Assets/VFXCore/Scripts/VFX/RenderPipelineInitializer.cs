/*========================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
=========================================================================*/
using UnityEngine;
using System.Collections;
using Vuforia;

namespace VFX
{
    /// <summary>
    /// A script that initializes the Unity Render Pipeline.
    /// Usage: this script can be added to an arbitrary game-object
    /// in the Unity Scene.
    /// </summary>
    public class RenderPipelineInitializer : MonoBehaviour
    {
        const float PRIMITIVE_SCALE = 0.001f;
        
        protected GameObject mPrimitive;


        void Start()
        {
            VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
        }

        void OnDestroy()
        {
            VuforiaApplication.Instance.OnVuforiaStarted -= OnVuforiaStarted;
        }

        void OnVuforiaStarted()
        {
            StartCoroutine(InitRenderPipeline());
        }

        IEnumerator InitRenderPipeline()
        {
            // Wait end of frame to make sure the Vuforia Camera
            // is completely set up.
            yield return new WaitForEndOfFrame();

            // Create a primitive with default Unity material;
            // this forces Unity to correctly initialize the render pipeline,
            // in case the Scenes only includes materials with custom shaders.
            // This is needed in Unity 2019, 2020 and 2021 versions,
            // as a countermeasure to the following Unity bug:
            // https://issuetracker.unity3d.com/issues/custom-shader-not-rendered-correctly-when-no-opaque-objects-are-in-the-scene
            mPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Position and scale the primitive relative to the camera
            var cam = VuforiaBehaviour.Instance.GetComponent<Camera>();
            mPrimitive.transform.SetParent(cam.transform, worldPositionStays: false);
            var depth = 0.5f * (cam.nearClipPlane + cam.farClipPlane);
            var size = PRIMITIVE_SCALE * depth;
            mPrimitive.transform.localPosition = depth * Vector3.forward;
            mPrimitive.transform.localScale = size * Vector3.one;
        }
    }
}
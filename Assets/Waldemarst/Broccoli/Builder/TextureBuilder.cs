﻿using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Broccoli.Builder
{
    public class TextureBuilder {
        #region Vars
        /// <summary>
		/// The size of the texture to generate.
		/// </summary>
		public Vector2 textureSize = new Vector2(1024, 1024);
        /// <summary>
        /// Shader to use to render the texture.
        /// </summary>
        public Shader shader = null;
        Plane _cameraPlane = new Plane (Vector3.forward, Vector3.zero);
        Vector3 _cameraUp = Vector3.up;
        Mesh _targetMesh = null;
        GameObject _targetGameObject = null;
        /// <summary>
		/// The camera rendering the texture.
		/// </summary>
		Camera camera = null;
		/// <summary>
		/// Game object containing the camera.
		/// </summary>
		GameObject cameraGameObject = null;
        /// <summary>
		/// Layer to move the object when rendering with the camera.
		/// </summary>
		private const int PREVIEW_LAYER = 22;
        /// <summary>
        /// Debug flag: the texture rendering camera does not get destroyed after usage.
        /// </summary>
        public bool debugKeepCameraAfterUsage = false;
        /// <summary>
        /// Background color for the camera.
        /// </summary>
        public Color backgroundColor = new Color (0.5f, 0.5f, 0.5f, 0f);
        #endregion  

        #region Accessors
        public Plane lastCameraPlane {
            get { return _cameraPlane; }
        }
        public Vector3 lastCameraUp {
            get { return _cameraUp; }
        }
        #endregion

        #region Render methods
        public void BeginUsage (GameObject target) {
            _targetGameObject = target;
            _targetMesh = _targetGameObject.GetComponent<MeshFilter> ().sharedMesh;
            SetupCamera ();
        }
        public void EndUsage () {
            _targetMesh = null;
            if (!debugKeepCameraAfterUsage) {
                RemoveCamera ();
            }
        }
        public Texture2D GetTexture (Plane cameraPlane, Vector3 cameraUp, string texturePath = "") {
            if (_targetGameObject == null) {
                Debug.LogWarning ("Target mesh not set on TextureBuilder.");
            } else {
                // Set params
                _cameraPlane = new Plane (cameraPlane.normal, _targetMesh.bounds.center);
                _cameraUp = Vector3.ProjectOnPlane (cameraUp, _cameraPlane.normal).normalized;

                // Prepare textures
                Texture2D targetTexture = new Texture2D ((int)textureSize.x, (int)textureSize.y, TextureFormat.ARGB32, true);
                RenderTexture renderTexture = RenderTexture.GetTemporary ((int)textureSize.x, (int)textureSize.y, 16, RenderTextureFormat.ARGB32);
                RenderTexture.active = renderTexture;

                // Prepare target
                int originalLayer = _targetGameObject.layer;
			    SetLayerRecursively (_targetGameObject.transform);

                _targetGameObject.hideFlags = HideFlags.None;
                _targetGameObject.SetActive (true);
                
                // Prepare camera
                PositionCamera ();
				camera.targetTexture = renderTexture;
                // Render without SRP, save the render pipeline to a temp var, then assign it back after rendering
                RenderPipelineAsset renderPipeline = GraphicsSettings.renderPipelineAsset;
                GraphicsSettings.renderPipelineAsset = null;

                // Render
                if (shader != null) {
					camera.RenderWithShader (shader, "");
				} else {
					camera.Render ();
				}

                // Write to texture
                targetTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
				targetTexture.Apply();

                // Assign back the render pipeline. TODO: assign back using a try-catch block
                GraphicsSettings.renderPipelineAsset = renderPipeline;

                // Restore target to its original Layer
                SetLayerRecursively (_targetGameObject.transform, originalLayer);

                // Cleanup
                RenderTexture.ReleaseTemporary (renderTexture);
                RenderTexture.active = null;

                #if UNITY_EDITOR
                if (!string.IsNullOrEmpty (texturePath)) {
                    var bytes = targetTexture.EncodeToPNG();
                    File.WriteAllBytes (texturePath, bytes);
                    targetTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D> (texturePath);
                    AssetDatabase.ImportAsset (texturePath);
                }
                #endif

                return targetTexture;
            }
            return null;
        }
		Shader GetUnlitShader () {
			return Shader.Find ("Broccoli/Billboard Unlit");
		}
		Shader GetNormalShader () {
			return Shader.Find ("Broccoli/Billboard Normals");
		}
        /// <summary>
		/// Sets the layer of an object recursively.
		/// </summary>
		/// <param name="obj">Object.</param>
		private static void SetLayerRecursively (Transform obj, int layer = -1) {
			if (layer < 0)
				layer = PREVIEW_LAYER;
			obj.gameObject.layer = layer;
			for( int i = 0; i < obj.childCount; i++ )
				SetLayerRecursively( obj.GetChild( i ) );
		}
        #endregion

        #region Camera methods
        /// <summary>
		/// Setups the camera.
		/// </summary>
		/// <param name="target">Target.</param>
		private void SetupCamera () {
			if (camera != null) {
				Object.DestroyImmediate (camera);
			}
			if (cameraGameObject != null) {
				Object.DestroyImmediate (cameraGameObject);
				cameraGameObject = null;
			}

			cameraGameObject = new GameObject ("TextureBuilderCameraContainer");
			camera = cameraGameObject.AddComponent<Camera> ();

			// Set camera properties
			camera.cameraType = CameraType.Preview;
			camera.clearFlags = CameraClearFlags.Color;
			camera.backgroundColor = backgroundColor;
			camera.cullingMask = 1 << PREVIEW_LAYER;
			camera.orthographic = true;
			camera.enabled = false;
        /// <summary>
		}
		/// Destroy the camera objects.
		/// </summary>
		public void RemoveCamera () {
			// Remove camera.
			if (camera != null)
				Object.DestroyImmediate (camera);
			if (cameraGameObject != null)
				Object.DestroyImmediate (cameraGameObject);
		}
        private void PositionCamera () {
			// Aspect and size of the camera.
            Bounds projectBounds = _targetMesh.bounds;
			camera.aspect = projectBounds.size.z / projectBounds.size.y;
            camera.orthographicSize = projectBounds.size.y / 2f;
			// Positioning the camera.
			float num = _targetMesh.bounds.extents.magnitude;
			camera.nearClipPlane = num * 0.1f;
			camera.farClipPlane = num * 2.2f;
			cameraGameObject.transform.position = _targetMesh.bounds.center + _targetGameObject.transform.position + _cameraPlane.normal * num;
			cameraGameObject.transform.LookAt (_targetMesh.bounds.center + _targetGameObject.transform.position, _cameraUp);
        }
        #endregion
    }
}
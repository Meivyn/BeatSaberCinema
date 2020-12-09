﻿using System;
using System.Linq;
using UnityEngine;

namespace BeatSaberCinema
{
	public class Screen: MonoBehaviour
	{
		private readonly GameObject _screenGameObject;
		private readonly GameObject _screenBodyGameObject;
		private readonly CurvedSurface _screenSurface;
		private readonly Renderer _screenRenderer;
		private CurvedSurface _screenBodySurface = null!;
		private readonly CustomBloomPrePass _screenBloomPrePass;

		public Screen()
		{
			_screenGameObject = new GameObject("CinemaScreen");
			_screenSurface = _screenGameObject.AddComponent<CurvedSurface>();
			_screenGameObject.layer = LayerMask.NameToLayer("Environment");
			_screenRenderer = _screenGameObject.GetComponent<Renderer>();
			_screenBodyGameObject = CreateBody();
			_screenBloomPrePass = _screenGameObject.AddComponent<CustomBloomPrePass>();

			Hide();
		}

		private GameObject CreateBody()
		{
			GameObject body = new GameObject("CinemaScreenBody");
			_screenBodySurface = body.AddComponent<CurvedSurface>();
			body.transform.parent = _screenGameObject.transform;
			body.transform.localPosition = new Vector3(0, 0, 0.025f);
			Renderer bodyRenderer = body.GetComponent<Renderer>();
			bodyRenderer.material = new Material(Resources.FindObjectsOfTypeAll<Material>()
				.Last(x => x.name == "DarkEnvironmentSimple"));
			body.layer = LayerMask.NameToLayer("Environment");
			return body;
		}

		public void Show()
		{
			_screenGameObject.SetActive(true);
		}

		public void Hide()
		{
			_screenGameObject.SetActive(false);
		}

		public void ShowBody()
		{
			Plugin.Logger.Debug("Showing body");
			_screenBodyGameObject.SetActive(true);
		}

		public void HideBody()
		{
			Plugin.Logger.Debug("Hiding body");
			_screenBodyGameObject.SetActive(false);
		}

		public Renderer GetRenderer()
		{
			return _screenRenderer;
		}

		public void SetTransform(Transform parentTransform)
		{
			_screenGameObject.transform.parent = parentTransform;
		}

		public void SetPlacement(Vector3 pos, Vector3 rot, float width, float height, float? curvatureDegrees)
		{
			_screenGameObject.transform.position = pos;
			_screenGameObject.transform.eulerAngles = rot;
			InitializeSurfaces(width, height, pos.z, curvatureDegrees);
		}

		public void InitializeSurfaces(float width, float height, float distance, float? curvatureDegrees)
		{
			_screenSurface.Initialize(width, height, distance, curvatureDegrees);
			_screenBodySurface.Initialize(width+0.15f, height+0.05f, distance+0.05f, curvatureDegrees);
		}

		public void RegenerateScreenSurfaces()
		{
			_screenSurface.Generate();
			_screenBodySurface.Generate();
			_screenBloomPrePass.UpdateMesh();
		}

		public void SetDistance(float distance)
		{
			var currentPos = _screenGameObject.transform.position;
			_screenGameObject.transform.position = new Vector3(currentPos.x, currentPos.y, distance);
			_screenSurface.Distance = distance;
			_screenBodySurface.Distance = distance;
		}

		public void SetAspectRatio(float ratio)
		{
			_screenSurface.Width = _screenSurface.Height * ratio;
			_screenBodySurface.Width = _screenSurface.Height * ratio;
			RegenerateScreenSurfaces();
		}
	}
}
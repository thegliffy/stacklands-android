using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Screenshotter : MonoBehaviour
{
	public static Screenshotter instance;

	[HideInInspector]
	public List<ScreenshotDescription> Descriptions = new List<ScreenshotDescription>();

	public bool IsScreenshotting;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		List<ScreenshotDescription> list = new List<ScreenshotDescription>();
		list.Add(new ScreenshotDescription(1920, 1080)
		{
			Description = "schinese",
			Language = "Chinese (Simplified)"
		});
		list.Add(new ScreenshotDescription(1920, 1080)
		{
			Description = "tchinese",
			Language = "Chinese (Traditional)"
		});
		list.Add(new ScreenshotDescription(1920, 1080)
		{
			Description = "koreana",
			Language = "Korean"
		});
		list.Add(new ScreenshotDescription(1920, 1080)
		{
			Description = "english",
			Language = "English"
		});
		Descriptions.AddRange(list);
	}

	private void LateUpdate()
	{
		if (InputController.instance.GetKeyDown(Key.F7))
		{
			StartCoroutine(TakeAllScreenshots());
		}
	}

	private IEnumerator TakeAllScreenshots()
	{
		DateTime curTime = DateTime.Now;
		foreach (ScreenshotDescription description in Descriptions)
		{
			if (description.IncludeInScreenshots)
			{
				description.TakenAt = curTime;
				yield return TakeScreenshot(description);
			}
		}
	}

	public IEnumerator TakeScreenshot(ScreenshotDescription sd)
	{
		IsScreenshotting = true;
		GameCanvas.instance.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
		GameCanvas.instance.Canvas.worldCamera = GameCamera.instance.MyCam;
		GameCanvas.instance.Canvas.sortingOrder = 2;
		GameCanvas.instance.Canvas.sortingLayerName = "Above";
		if (sd.ShowUI)
		{
			GameCanvas.instance.SetUIToggle(enabled: true);
		}
		else
		{
			GameCanvas.instance.SetUIToggle(enabled: false);
		}
		string originalLanguage = SokLoc.instance.CurrentLanguage;
		SokLoc.instance.SetLanguage(sd.Language);
		if (sd.ControlSchemeOverride.HasValue)
		{
			InputController.instance.SchemeOverride = sd.ControlSchemeOverride;
		}
		for (int i = 0; i < 5; i++)
		{
			Canvas.ForceUpdateCanvases();
		}
		if (GameCanvas.instance != null)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(GameCanvas.instance.transform as RectTransform);
		}
		yield return null;
		yield return new WaitForEndOfFrame();
		MakeScreenshot(sd, out var success, out var targetPath);
		GameCanvas.instance.SetUIToggle(enabled: true);
		Canvas.ForceUpdateCanvases();
		SokLoc.instance.SetLanguage(originalLanguage);
		GameCanvas.instance.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		InputController.instance.SchemeOverride = null;
		if (success)
		{
			Debug.Log("Screenshot saved to " + targetPath);
		}
		IsScreenshotting = false;
	}

	public static void MakeScreenshot(ScreenshotDescription sd, out bool success, out string targetPath)
	{
		RenderTextureDescriptor desc = new RenderTextureDescriptor(sd.Width, sd.Height, RenderTextureFormat.ARGB32, 32);
		desc.sRGB = true;
		desc.stencilFormat = GraphicsFormat.R8_UInt;
		RenderTexture temporary = RenderTexture.GetTemporary(desc);
		temporary.antiAliasing = 8;
		Camera.main.targetTexture = temporary;
		Camera.main.Render();
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		Texture2D texture2D = new Texture2D(temporary.width, temporary.height, sd.AlphaBackground ? TextureFormat.ARGB32 : TextureFormat.RGB24, mipChain: false, linear: true);
		texture2D.ReadPixels(new Rect(0f, 0f, temporary.width, temporary.height), 0, 0);
		texture2D.Apply();
		WriteTexture(texture2D, sd, out success, out targetPath);
		Camera.main.targetTexture = null;
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
	}

	private static void WriteTexture(Texture2D tex, ScreenshotDescription desc, out bool success, out string targetPath)
	{
		string text = Path.Combine(Application.persistentDataPath, "screenshots");
		Directory.CreateDirectory(text);
		success = true;
		string path = $"{desc.TakenAt.ToFileTimeUtc()} {desc.Description}.png";
		targetPath = Path.Combine(text, path);
		try
		{
			File.WriteAllBytes(targetPath, tex.EncodeToPNG());
		}
		catch (Exception arg)
		{
			Debug.LogError($"Saving screenshot failed\n{arg}");
			success = false;
		}
	}
}

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class ResourceHelper
{
	public static Sprite LoadSpriteFromPath(string path)
	{
		Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGBA32, mipChain: false);
		texture2D.LoadImage(File.ReadAllBytes(path));
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.one / 2f);
	}

	public static IEnumerator LoadAudioClipFromPath(string path, Action<AudioClip> callback, Action onError = null)
	{
		return LoadAudioClipFromPath(path, AudioType.WAV, callback, onError);
	}

	public static IEnumerator LoadAudioClipFromPath(string path, AudioType type, Action<AudioClip> callback, Action onError = null)
	{
		using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, type);
		www.timeout = 3;
		yield return www.SendWebRequest();
		if (www.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.LogWarning("Error while loading audio from " + path + ": " + www.error);
			onError?.Invoke();
		}
		else
		{
			callback(DownloadHandlerAudioClip.GetContent(www));
		}
	}
}

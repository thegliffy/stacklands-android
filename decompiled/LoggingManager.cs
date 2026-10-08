using System;
using System.Collections.Generic;
using ImGuiNET;
using UImGui;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoggingManager : MonoBehaviour
{
	public static List<(int, string)> Logs = new List<(int, string)>();

	public static LoggingManager instance;

	public bool LogViewerEnabled;

	private void Awake()
	{
		instance = this;
		Application.logMessageReceived += HandleLog;
		UImGuiUtility.Layout += DrawLogViewer;
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (!logString.StartsWith("["))
		{
			logString = string.Format("[{0}] [{1} : Stacklands] {2}{3}", DateTime.Now.ToString("HH:mm:ss"), type, logString, (type == LogType.Exception) ? ("\n" + stackTrace) : "");
		}
		Logs.Add(((int)type, logString));
	}

	private void Update()
	{
		if (Keyboard.current[Key.F3].wasPressedThisFrame)
		{
			LogViewerEnabled = !LogViewerEnabled;
		}
	}

	private void DrawLogViewer(global::UImGui.UImGui _)
	{
		if (!LogViewerEnabled)
		{
			return;
		}
		ImGui.SetNextWindowSize(new Vector2(520f, 600f), ImGuiCond.FirstUseEver);
		ImGui.Begin("Log");
		if (ImGui.Button("Open log file"))
		{
			Application.OpenURL("file:///" + Application.consoleLogPath);
		}
		ImGui.SameLine();
		if (ImGui.Button("Clear log"))
		{
			Logs.Clear();
		}
		ImGui.Separator();
		if (ImGui.BeginChild("logsection", new Vector2(0f, 0f - (ImGui.GetStyle().ItemSpacing.y + ImGui.GetFrameHeightWithSpacing())), border: false, ImGuiWindowFlags.HorizontalScrollbar))
		{
			ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4f, 1f));
			foreach (var log in Logs)
			{
				int item = log.Item1;
				string item2 = log.Item2;
				bool flag = false;
				Vector4 col = Vector4.zero;
				if (item == 2)
				{
					flag = true;
					col = new Vector4(1f, 1f, 0f, 1f);
				}
				if (item == 0 || item == 4)
				{
					flag = true;
					col = new Vector4(1f, 0f, 0f, 1f);
				}
				if (flag)
				{
					ImGui.PushStyleColor(ImGuiCol.Text, col);
				}
				ImGui.TextUnformatted(item2);
				if (flag)
				{
					ImGui.PopStyleColor();
				}
			}
			ImGui.PopStyleVar();
		}
		ImGui.EndChild();
		ImGui.End();
	}
}

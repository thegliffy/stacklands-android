using System.IO;
using ImGuiNET;
using UImGui;
using UnityEngine;
using UnityEngine.InputSystem;

public class SchemaUI : MonoBehaviour
{
	private bool Show;

	private string FolderInput = "";

	private void Start()
	{
		UImGuiUtility.Layout += DrawSchemaUI;
	}

	private void Update()
	{
		if (Keyboard.current[Key.F6].wasPressedThisFrame)
		{
			Show = !Show;
		}
	}

	private void DrawSchemaUI(global::UImGui.UImGui _)
	{
		if (Show)
		{
			ImGui.SetNextWindowSize(new Vector2(700f, 100f), ImGuiCond.Once);
			ImGui.Begin("JSON Schema Generator");
			ImGui.InputTextWithHint("Schema Folder", "C:/Users/cyber/Documents/stacklands-schemas", ref FolderInput, 500u);
			if (ImGui.Button("Generate card.schema.json"))
			{
				SchemaGenerator.GenerateCardSchema(Path.Combine(FolderInput, "card.schema.json"));
			}
			ImGui.SameLine();
			if (ImGui.Button("Generate blueprint.schema.json"))
			{
				SchemaGenerator.GenerateBlueprintSchema(Path.Combine(FolderInput, "blueprint.schema.json"));
			}
			ImGui.SameLine();
			if (ImGui.Button("Generate boosterpack.schema.json"))
			{
				SchemaGenerator.GenerateBoosterSchema(Path.Combine(FolderInput, "boosterpack.schema.json"));
			}
			if (ImGui.Button("Generate ALL schemas"))
			{
				SchemaGenerator.GenerateSchemas(FolderInput);
			}
			ImGui.End();
		}
	}
}

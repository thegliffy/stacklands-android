using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindElement : MonoBehaviour
{
	public string MyAction;

	public string Scheme;

	public TextMeshProUGUI ActionName;

	public CustomButton ResetButton;

	public CustomButton SetBindingButton;

	private void Start()
	{
		if (!ResolveActionAndBinding(out var action, out var bindingIndex))
		{
			return;
		}
		SetBindingButton.Clicked += delegate
		{
			if (!ControlsScreen.instance.IsRebinding)
			{
				StartRebind();
			}
		};
		ResetButton.Clicked += delegate
		{
			if (action.bindings[bindingIndex].isComposite)
			{
				for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
				{
					action.RemoveBindingOverride(i);
				}
			}
			else
			{
				action.RemoveBindingOverride(bindingIndex);
			}
			ControlsScreen.instance.SaveRebinds();
		};
	}

	public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
	{
		bindingIndex = -1;
		action = InputController.instance.PlayerInput.actions[MyAction];
		for (int i = 0; i < action.bindings.Count; i++)
		{
			InputBinding inputBinding = action.bindings[i];
			if (inputBinding.isComposite)
			{
				if (action.bindings[i + 1].groups.Contains(Scheme))
				{
					bindingIndex = i;
					return true;
				}
			}
			else if (inputBinding.groups.Contains(Scheme))
			{
				bindingIndex = i;
				return true;
			}
		}
		Debug.LogError("No action found for " + MyAction + " in " + Scheme);
		return false;
	}

	private void StartRebind()
	{
		if (!ResolveActionAndBinding(out var action, out var bindingIndex))
		{
			return;
		}
		if (action.bindings[bindingIndex].isComposite)
		{
			int num = bindingIndex + 1;
			if (num < action.bindings.Count && action.bindings[num].isPartOfComposite)
			{
				Rebind(action, num, isComposite: true);
			}
		}
		else
		{
			Rebind(action, bindingIndex);
		}
	}

	private void Rebind(InputAction action, int bindingIndex, bool isComposite = false)
	{
		action.Disable();
		InputActionRebindingExtensions.RebindingOperation rebindOperation = action.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough("<Keyboard>/escape").OnMatchWaitForAnother(0.1f)
			.WithControlsExcluding("<Mouse>/leftButton");
		rebindOperation.OnComplete(delegate
		{
			if (CheckDuplicateBinding(action, bindingIndex))
			{
				action.RemoveBindingOverride(bindingIndex);
			}
			rebindOperation?.Dispose();
			rebindOperation = null;
			action.Enable();
			ControlsScreen.instance.RebindInfo = null;
			ControlsScreen.instance.SaveRebinds();
			if (isComposite)
			{
				int num = bindingIndex + 1;
				if (num < action.bindings.Count && action.bindings[num].isPartOfComposite)
				{
					Rebind(action, num, isComposite: true);
				}
			}
			InputController.instance.ClearBindingDisplayCache();
		});
		rebindOperation.OnCancel(delegate
		{
			rebindOperation?.Dispose();
			rebindOperation = null;
			ControlsScreen.instance.RebindInfo = null;
			action.Enable();
		});
		ControlsScreen.instance.RebindInfo = new RebindInfo
		{
			Action = action,
			BindingIndex = bindingIndex
		};
		rebindOperation.Start();
	}

	private bool CheckDuplicateBinding(InputAction action, int bindingIndex)
	{
		InputBinding inputBinding = action.bindings[bindingIndex];
		foreach (InputBinding binding in action.actionMap.bindings)
		{
			if (!(binding.action == inputBinding.action) && binding.effectivePath == inputBinding.effectivePath)
			{
				return true;
			}
		}
		if (inputBinding.isPartOfComposite)
		{
			for (int i = 1; i < bindingIndex; i++)
			{
				if (action.bindings[i].overridePath == inputBinding.overridePath)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void Update()
	{
		if (ResolveActionAndBinding(out var action, out var bindingIndex))
		{
			ActionName.text = SokLoc.Translate("control_" + action.name);
			SetBindingButton.TextMeshPro.text = action.GetBindingDisplayString(bindingIndex);
		}
	}
}

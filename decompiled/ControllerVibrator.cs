using UnityEngine.InputSystem;

public class ControllerVibrator
{
	public VibratePattern CurrentPattern;

	public int curVibrateIndex;

	private float curVibrateTimer;

	public void StartVibrate(VibratePattern pattern)
	{
		CurrentPattern = pattern;
		curVibrateTimer = 0f;
		curVibrateIndex = 0;
		Gamepad.current?.SetMotorSpeeds(CurrentPattern.LowFrequencies[curVibrateIndex], CurrentPattern.HighFrequencies[curVibrateIndex]);
	}

	public void UpdateVibrate(float deltaTime)
	{
		if (CurrentPattern == null)
		{
			return;
		}
		if (InputController.instance.CurrentScheme != ControlScheme.Controller || Gamepad.current == null)
		{
			StopVibrate();
			return;
		}
		Gamepad.current?.SetMotorSpeeds(CurrentPattern.LowFrequencies[curVibrateIndex], CurrentPattern.HighFrequencies[curVibrateIndex]);
		curVibrateTimer += deltaTime;
		if (curVibrateTimer >= CurrentPattern.Times[curVibrateIndex])
		{
			curVibrateTimer -= CurrentPattern.Times[curVibrateIndex];
			curVibrateIndex++;
			if (curVibrateIndex >= CurrentPattern.Times.Count)
			{
				StopVibrate();
			}
		}
	}

	public void StopVibrate()
	{
		CurrentPattern = null;
		Gamepad.current?.SetMotorSpeeds(0f, 0f);
	}
}

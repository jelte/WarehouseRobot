using TMPro;
using UnityEngine;

public class CommandQueue : MonoBehaviour
{
	public TextMeshProUGUI commandQueueText;
	
	public void Initialize(RobotController controller)
	{
		controller.commandReceivedEvent += OnCommandAdded;
	}
	
	private void OnCommandAdded(Command command)
	{
		if (command == Command.Execute)
		{
			commandQueueText.text = string.Empty;
			return;
		}
		commandQueueText.text += $"{command}\n";
	}
}
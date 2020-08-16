using UnityEngine;

// 
// -X: West
// +X: East
// +Y: North
// -Y: South

public class Main : MonoBehaviour
{
	private Grid grid;
	private RobotController controller;
	
	public CommandQueue commandQueue;
	
	[Header("Prefabs")]
	public Robot robotPrefab;
	public Crate cratePrefab;
	
	void Awake()
	{
		grid = new Grid(new BoundsInt(Vector3Int.zero, Vector3Int.one * 10));
		controller = new RobotController(grid);
		commandQueue.Initialize(controller);
		
		controller.SetRobot(SpawnRobot(Vector3Int.zero));

		
		SpawnCrate(new Vector3Int(4, 0, 4));
		SpawnCrate(new Vector3Int(9, 0, 9));
	}

	private Robot SpawnRobot(Vector3Int position)
	{
		Robot robot = GameObject.Instantiate(robotPrefab, position, Quaternion.identity);
		robot.Position = position;
		return robot;
	}

	private void SpawnCrate(Vector3Int position)
	{		
		Crate crate = GameObject.Instantiate(cratePrefab, position, Quaternion.identity);
		crate.Position = position;
		grid.Add(crate);
	}
	
	void Update()
	{
		if (Input.GetButtonDown(nameof(Command.North)))
		{
			controller.AddCommand(Command.North);
		}
		if (Input.GetButtonDown(nameof(Command.South)))
		{
			controller.AddCommand(Command.South);
		}
		if (Input.GetButtonDown(nameof(Command.East)))
		{
			controller.AddCommand(Command.East);
		}
		if (Input.GetButtonDown(nameof(Command.West)))
		{
			controller.AddCommand(Command.West);
		}
		if (Input.GetButtonDown(nameof(Command.Grab)))
		{
			controller.AddCommand(Command.Grab);
		}
		if (Input.GetButtonDown(nameof(Command.Drop)))
		{
			controller.AddCommand(Command.Drop);
		}
		if (Input.GetButtonDown(nameof(Command.Execute)))
		{
			controller.AddCommand(Command.Execute);
		}
	}
}

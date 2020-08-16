using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController
{
	public event Action<Command> commandReceivedEvent;
	
	private Grid grid;
	private Queue<Command> pendingCommandQueue = new Queue<Command>();
	private Queue<Command> activeCommandQueue;
	
	private Robot robot;
	
	public RobotController(Grid grid)
	{
		this.grid = grid;
	}

	public void SetRobot(Robot robot)
	{
		this.robot = robot;
	}
	
	public void AddCommand(Command command)
	{
		if (command == Command.Execute)
		{
			Execute();
			return;
		}
		pendingCommandQueue.Enqueue(command);
		commandReceivedEvent?.Invoke(command);
	}

	private void Execute()
	{
		if (activeCommandQueue != null)
		{
			return;
		}

		commandReceivedEvent?.Invoke(Command.Execute);
		activeCommandQueue = pendingCommandQueue;
		pendingCommandQueue = new Queue<Command>();
		robot.StartCoroutine(ExecuteCommands());
	}

	private IEnumerator ExecuteCommands()
	{
		while (activeCommandQueue.Count > 0)
		{
			Command command = activeCommandQueue.Dequeue();
			Command? nextCommand = null;
			if (activeCommandQueue.Count > 0)
			{
				nextCommand = activeCommandQueue.Peek();
			}
			yield return ExecuteCommand(command, nextCommand);
		}

		activeCommandQueue = null;
	}

	private IEnumerator ExecuteCommand(Command command, Command? nextCommand)
	{
		switch (command)
		{
			case Command.Grab:
				yield return TryGrab();
				break;
			case Command.Drop:
				yield return TryDrop();
				break;
			case Command.North:
			case Command.South:
				yield return Move(command, nextCommand == Command.East || nextCommand == Command.West ? nextCommand : null);
				break;
			case Command.East:
			case Command.West:
				yield return Move(command, nextCommand == Command.North || nextCommand == Command.South ? nextCommand : null);
				break;
		}
	}

	private IEnumerator Move(Command command, Command? nextCommand)
	{
		Vector3Int movement = GetMovement(command);
		float angle = GetAngle(command);
		if (nextCommand.HasValue)
		{
			Vector3Int nextMovement = GetMovement(nextCommand.Value);
			if (grid.IsAvailable(robot.Position + movement + nextMovement))
			{
				movement += nextMovement;
				angle = GetAngle(nextCommand.Value);
				activeCommandQueue.Dequeue();	
			}
		}
		return TryMove(movement, angle);
	}

	private Vector3Int GetMovement(Command command)
	{
		switch (command)
		{
			case Command.North:
				return new Vector3Int(0, 0, 1);
			case Command.East:
				return Vector3Int.right;
			case Command.South:
				return new Vector3Int(0, 0, -1);
			case Command.West:
				return Vector3Int.left;
		}
		return Vector3Int.zero;
	}

	private float GetAngle(Command command)
	{
		switch (command)
		{
			case Command.North:
				return 0f;
			case Command.East:
				return 90f;
			case Command.South:
				return 180f;
			case Command.West:
				return 270f;
		}
		return 0;
	}

	private IEnumerator TryMove(Vector3Int movement, float angle)
	{
		Vector3Int target = robot.Position + movement;
		robot.RotateToward(angle);
		yield return new WaitUntil(robot.FinishedRotating);
		robot.Forward = movement;
		
		if (!grid.IsAvailable(target))
		{
			yield break;
		}

		robot.MoveTo(target);
		yield return new WaitUntil(robot.FinishedMove);
	}
	
	private IEnumerator TryGrab()
	{
		if (robot.HasCrate)
		{
			yield break;
		}

		Crate crate = grid.Get(robot.Forward) as Crate;
		if (crate == null)
		{
			yield break;
		}

		grid.Remove(crate);
		robot.Grab(crate);
		yield return new WaitUntil(robot.CrateLifted);
	}
	
	private IEnumerator TryDrop()
	{
		if (!robot.HasCrate || !grid.IsAvailable(robot.Forward))
		{
			yield break;
		}

		Crate crate = robot.Drop();
		yield return new WaitUntil(robot.CrateDropped);
		crate.transform.parent = null;
		crate.Position = robot.Forward;
		grid.Add(crate);
	}
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid
{
	private List<IGridObject> grid;
	private BoundsInt bounds;

	public Grid(BoundsInt bounds)
	{
		this.bounds = bounds;
		grid = new List<IGridObject>();
	}

	public void Add(IGridObject gridObject)
	{
		grid.Add(gridObject);
	}
	
	public void Remove(IGridObject gridObject)
	{
		grid.Remove(gridObject);
	}

	public bool IsAvailable(Vector3Int target)
	{
		if (!bounds.Contains(target))
		{
			return false;
		}

		return !grid.Any(gridObject => gridObject != null && gridObject.Position == target);
	}

	public IGridObject Get(Vector3Int target)
	{
		return grid.SingleOrDefault(gridObject => gridObject.Position == target);
	}
}
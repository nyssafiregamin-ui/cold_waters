using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class Path_AStar
{
	// Token: 0x060008DB RID: 2267 RVA: 0x000647A8 File Offset: 0x000629A8
	public Path_AStar(string start, string goal)
	{
		if (UIFunctions.globaluifunctions.campaignmanager.tileGraph == null)
		{
			UIFunctions.globaluifunctions.campaignmanager.tileGraph = new Path_TileGraph();
		}
		if (!UIFunctions.globaluifunctions.campaignmanager.tileGraph.nodes.ContainsKey(start))
		{
			start = this.GetNearestWalkableTile(start);
			if (start == string.Empty)
			{
				return;
			}
		}
		if (!UIFunctions.globaluifunctions.campaignmanager.tileGraph.nodes.ContainsKey(goal))
		{
			goal = this.GetNearestWalkableTile(goal);
			if (goal == string.Empty)
			{
				return;
			}
		}
		List<string> list = new List<string>();
		SimplePriorityQueue<string> simplePriorityQueue = new SimplePriorityQueue<string>();
		simplePriorityQueue.Enqueue(start, 0.0);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, float> dictionary2 = new Dictionary<string, float>();
		foreach (string key in UIFunctions.globaluifunctions.campaignmanager.tileGraph.nodes.Keys)
		{
			dictionary2[key] = float.PositiveInfinity;
		}
		dictionary2[start] = 0f;
		Dictionary<string, float> dictionary3 = new Dictionary<string, float>();
		foreach (string key2 in UIFunctions.globaluifunctions.campaignmanager.tileGraph.nodes.Keys)
		{
			dictionary3[key2] = float.PositiveInfinity;
		}
		dictionary3[start] = this.heuristic_cost_estimate(start, goal);
		while (simplePriorityQueue.Count > 0)
		{
			string text = simplePriorityQueue.Dequeue();
			if (text == goal)
			{
				this.reconstruct_path(dictionary, text);
				return;
			}
			list.Add(text);
			foreach (string text2 in UIFunctions.globaluifunctions.campaignmanager.tileGraph.nodes[text])
			{
				if (!list.Contains(text2))
				{
					float num = this.dist_between(text, text2);
					float num2 = dictionary2[text] + num;
					if (!simplePriorityQueue.Contains(text2) || num2 < dictionary2[text2])
					{
						dictionary[text2] = text;
						dictionary2[text2] = num2;
						dictionary3[text2] = dictionary2[text2] + this.heuristic_cost_estimate(text2, goal);
						if (!simplePriorityQueue.Contains(text2))
						{
							simplePriorityQueue.Enqueue(text2, (double)dictionary3[text2]);
						}
						else
						{
							simplePriorityQueue.UpdatePriority(text2, (double)dictionary3[text2]);
						}
					}
				}
			}
		}
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x00064AD8 File Offset: 0x00062CD8
	private float heuristic_cost_estimate(string a, string b)
	{
		string[] array = a.Split(new char[]
		{
			','
		});
		string[] array2 = b.Split(new char[]
		{
			','
		});
		return Mathf.Sqrt(Mathf.Pow((float)(int.Parse(array[0]) - int.Parse(array2[0])), 2f) + Mathf.Pow((float)(int.Parse(array[1]) - int.Parse(array2[1])), 2f));
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00064B48 File Offset: 0x00062D48
	private float dist_between(string a, string b)
	{
		string[] array = a.Split(new char[]
		{
			','
		});
		string[] array2 = b.Split(new char[]
		{
			','
		});
		if (Mathf.Abs(int.Parse(array[0]) - int.Parse(array2[0])) + Mathf.Abs(int.Parse(array[1]) - int.Parse(array2[1])) == 1)
		{
			return 1f;
		}
		if (Mathf.Abs(int.Parse(array[0]) - int.Parse(array2[0])) == 1 && Mathf.Abs(int.Parse(array[1]) - int.Parse(array2[1])) == 1)
		{
			return 1.4142135f;
		}
		return Mathf.Sqrt(Mathf.Pow((float)(int.Parse(array[0]) - int.Parse(array2[0])), 2f) + Mathf.Pow((float)(int.Parse(array[1]) - int.Parse(array2[1])), 2f));
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00064C30 File Offset: 0x00062E30
	private void reconstruct_path(Dictionary<string, string> cameFrom, string current)
	{
		Queue<string> queue = new Queue<string>();
		queue.Enqueue(current);
		while (cameFrom.ContainsKey(current))
		{
			current = cameFrom[current];
			queue.Enqueue(current);
		}
		this.path = new Queue<string>(queue.Reverse<string>());
		if (this.Length() > 2)
		{
			this.path.Dequeue();
		}
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x00064C94 File Offset: 0x00062E94
	public string Dequeue()
	{
		return this.path.Dequeue();
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00064CA4 File Offset: 0x00062EA4
	public int Length()
	{
		if (this.path == null)
		{
			return 0;
		}
		return this.path.Count;
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00064CC0 File Offset: 0x00062EC0
	private string GetNearestWalkableTile(string tile)
	{
		string empty = string.Empty;
		string[] array = tile.Split(new char[]
		{
			','
		});
		List<string> walkableNeighbours = UIFunctions.globaluifunctions.campaignmanager.tileGraph.GetWalkableNeighbours(int.Parse(array[0]), int.Parse(array[1]));
		if (walkableNeighbours.Count > 0)
		{
			return walkableNeighbours[UnityEngine.Random.Range(0, walkableNeighbours.Count)];
		}
		return empty;
	}

	// Token: 0x04000DA9 RID: 3497
	private Queue<string> path;

	// Token: 0x04000DAA RID: 3498
	public float timer;
}

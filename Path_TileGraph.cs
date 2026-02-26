using System;
using System.Collections.Generic;

// Token: 0x02000141 RID: 321
public class Path_TileGraph
{
	// Token: 0x060008E2 RID: 2274 RVA: 0x00064D2C File Offset: 0x00062F2C
	public Path_TileGraph()
	{
		this.nodes = new Dictionary<string, List<string>>();
		for (int i = 0; i < UIFunctions.globaluifunctions.campaignmanager.mapNavigation.width; i++)
		{
			for (int j = 0; j < UIFunctions.globaluifunctions.campaignmanager.mapNavigation.height; j++)
			{
				if (this.GetWalkablePixelAtCoord(i, j))
				{
					this.nodes.Add(i.ToString() + "," + j.ToString(), this.GetWalkableNeighbours(i, j));
				}
			}
		}
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00064DCC File Offset: 0x00062FCC
	public List<string> GetWalkableNeighbours(int x, int y)
	{
		List<string> list = new List<string>();
		if (this.GetWalkablePixelAtCoord(x, y + 1))
		{
			list.Add(x.ToString() + "," + (y + 1).ToString());
		}
		if (this.GetWalkablePixelAtCoord(x + 1, y + 1))
		{
			list.Add((x + 1).ToString() + "," + (y + 1).ToString());
		}
		if (this.GetWalkablePixelAtCoord(x + 1, y))
		{
			list.Add((x + 1).ToString() + "," + y.ToString());
		}
		if (this.GetWalkablePixelAtCoord(x + 1, y - 1))
		{
			list.Add((x + 1).ToString() + "," + (y - 1).ToString());
		}
		if (this.GetWalkablePixelAtCoord(x, y - 1))
		{
			list.Add(x.ToString() + "," + (y - 1).ToString());
		}
		if (this.GetWalkablePixelAtCoord(x - 1, y - 1))
		{
			list.Add((x - 1).ToString() + "," + (y - 1).ToString());
		}
		if (this.GetWalkablePixelAtCoord(x - 1, y))
		{
			list.Add((x - 1).ToString() + "," + y.ToString());
		}
		if (this.GetWalkablePixelAtCoord(x - 1, y + 1))
		{
			list.Add((x - 1).ToString() + "," + (y + 1).ToString());
		}
		return list;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x00064F8C File Offset: 0x0006318C
	private bool GetWalkablePixelAtCoord(int x, int y)
	{
		float r = UIFunctions.globaluifunctions.campaignmanager.mapNavigation.GetPixel(x, y).r;
		return r <= 0.3f;
	}

	// Token: 0x04000DAB RID: 3499
	public Dictionary<string, List<string>> nodes;
}

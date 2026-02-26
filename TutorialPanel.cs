using System;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class TutorialPanel : MonoBehaviour
{
	// Token: 0x06000ACE RID: 2766 RVA: 0x00096230 File Offset: 0x00094430
	private void Start()
	{
		this.currentPage = 0;
		for (int i = 1; i < this.tutorialPages.Length; i++)
		{
			this.tutorialPages[i].SetActive(false);
		}
		this.tutorialButtons[1].SetActive(false);
		this.tutorialPages[0].SetActive(true);
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x00096288 File Offset: 0x00094488
	private void NextPage()
	{
		this.tutorialPages[this.currentPage].SetActive(false);
		this.currentPage++;
		if (this.currentPage == this.tutorialPages.Length)
		{
			this.currentPage = 0;
		}
		this.tutorialPages[this.currentPage].SetActive(true);
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x000962E4 File Offset: 0x000944E4
	private void PrevPage()
	{
		this.tutorialPages[this.currentPage].SetActive(false);
		this.currentPage--;
		if (this.currentPage < 0)
		{
			this.currentPage = this.tutorialPages.Length - 1;
		}
		this.tutorialPages[this.currentPage].SetActive(true);
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00096344 File Offset: 0x00094544
	private void TutorialOff()
	{
		this.tutorialButtons[2].SetActive(false);
		this.tutorialButtons[1].SetActive(true);
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00096364 File Offset: 0x00094564
	private void TutorialOn()
	{
		this.tutorialButtons[2].SetActive(true);
		this.tutorialButtons[1].SetActive(false);
	}

	// Token: 0x040010DA RID: 4314
	public GameObject[] tutorialPages;

	// Token: 0x040010DB RID: 4315
	public int currentPage;

	// Token: 0x040010DC RID: 4316
	public GameObject[] tutorialButtons;
}

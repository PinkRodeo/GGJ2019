using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftPortalController : MonoBehaviour
{
	public SpriteRenderer stage1;
	public SpriteRenderer stage2;
	public SpriteRenderer stage3;
	public SpriteRenderer stage4;

	private void ShowCorrectPortal()
	{
		stage1.gameObject.SetActive(false);
		stage2.gameObject.SetActive(false);
		stage3.gameObject.SetActive(false);
		stage4.gameObject.SetActive(false);

		var state = JunkerGameMode.junkerState;

		int progress = 0;

		if (state.x_Recovered)
			progress += 1;
		if (state.y_Recovered)
			progress += 1;
		if (state.z_Recovered)
			progress += 1;

		if (progress == 0)
			stage1.gameObject.SetActive(true);
		if (progress == 1)
			stage2.gameObject.SetActive(true);
		if (progress == 2)
			stage3.gameObject.SetActive(true);
		if (progress == 3)
			stage4.gameObject.SetActive(true);

	}

	// Start is called before the first frame update
	void Start()
    {
		ShowCorrectPortal();
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}

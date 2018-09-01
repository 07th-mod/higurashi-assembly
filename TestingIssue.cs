using UnityEngine;

public class TestingIssue : MonoBehaviour
{
	private LTDescr lt;

	private LTDescr ff;

	private int id;

	private int fid;

	private void Start()
	{
		LeanTween.init();
		lt = LeanTween.move(base.gameObject, 100f * Vector3.one, 2f);
		id = lt.id;
		LeanTween.pause(id);
		ff = LeanTween.move(base.gameObject, Vector3.zero, 2f);
		fid = ff.id;
		LeanTween.pause(fid);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			LeanTween.resume(id);
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			LeanTween.resume(fid);
		}
	}
}

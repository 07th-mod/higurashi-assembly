using UnityEngine;

public class TestingIssue2 : MonoBehaviour
{
	public RectTransform rect;

	public GameObject go;

	private void Start()
	{
		int id = LeanTween.move(go, new Vector3(-0.209f, -3.891f, -1.162f), 2f).setEase(LeanTweenType.easeInQuart).id;
		int id2 = LeanTween.rotate(rect, 360f, 1f).id;
		Debug.Log("id1:" + id + " 2:" + id2);
		LeanTween.value(base.gameObject, Color.red, Color.green, 1f).setOnUpdate(OnTweenUpdate).setOnUpdateParam(new object[1]
		{
			string.Empty + 2
		});
	}

	private void OnTweenUpdate(Color update, object obj)
	{
		object[] array = obj as object[];
		Debug.Log("update:" + update + " obj:" + array[0]);
	}

	private void Update()
	{
	}
}

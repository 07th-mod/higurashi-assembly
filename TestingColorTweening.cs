using UnityEngine;

public class TestingColorTweening : MonoBehaviour
{
	private void Start()
	{
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
}

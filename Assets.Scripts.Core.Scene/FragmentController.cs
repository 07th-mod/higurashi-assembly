using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class FragmentController
	{
		public bool IsActive;

		private Cubemap cubemap;

		private string cubemapName;

		private string fragementPrefab;

		private GameObject fragment;

		private MeshRenderer mr;

		private LTDescr tween;

		public void CreateFragment(string cname, string pname, float time)
		{
			if (IsActive || fragment != null)
			{
				Terminate();
			}
			GameSystem.Instance.RegisterAction(delegate
			{
				cubemapName = cname;
				fragementPrefab = pname;
				fragment = Object.Instantiate(Resources.Load<GameObject>(fragementPrefab));
				mr = fragment.GetComponent<MeshRenderer>();
				cubemap = GameSystem.Instance.AssetManager.LoadCubemap(cubemapName);
				mr.sharedMaterial.SetTexture("_RefractTex", cubemap);
				mr.sharedMaterial.SetFloat("_Alpha", 0f);
				GameSystem.Instance.RegisterAction(delegate
				{
					tween = LeanTween.value(fragment, delegate(float f)
					{
						mr.sharedMaterial.SetFloat("_Alpha", f);
					}, 0f, 1f, time);
				});
				IsActive = true;
			});
		}

		public void StopFragment(float time)
		{
			if (fragment == null || !IsActive)
			{
				Terminate();
				return;
			}
			LeanTween.cancel(fragment);
			IsActive = false;
			if (Mathf.Approximately(time, 0f))
			{
				Terminate();
				return;
			}
			tween = LeanTween.value(fragment, delegate(float f)
			{
				mr.sharedMaterial.SetFloat("_Alpha", f);
			}, mr.sharedMaterial.GetFloat("_Alpha"), 0f, time);
			tween.onComplete = Terminate;
		}

		public void Terminate()
		{
			if (fragment != null)
			{
				LeanTween.cancel(fragment);
				Object.Destroy(fragment);
			}
			if (cubemap != null)
			{
				Object.Destroy(cubemap);
			}
			fragment = null;
			cubemap = null;
			fragementPrefab = null;
			cubemapName = null;
		}

		public void Serialize(MemoryStream ms)
		{
			BinaryWriter binaryWriter = new BinaryWriter(ms);
			binaryWriter.Write(IsActive);
			if (IsActive)
			{
				binaryWriter.Write(cubemapName);
				binaryWriter.Write(fragementPrefab);
			}
		}

		public void Deserialize(MemoryStream ms)
		{
			BinaryReader binaryReader = new BinaryReader(ms);
			if (IsActive)
			{
				Terminate();
			}
			IsActive = binaryReader.ReadBoolean();
			if (IsActive)
			{
				cubemapName = binaryReader.ReadString();
				fragementPrefab = binaryReader.ReadString();
				CreateFragment(cubemapName, fragementPrefab, 1f);
			}
		}
	}
}

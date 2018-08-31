using System;
using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	[Serializable]
	public class MtnCtrlElement
	{
		public int Points;

		public int Angle;

		public int Transparancy;

		public int StyleOfMovement;

		public int StyleOfRotation;

		public int Time;

		public Vector3[] Route = new Vector3[16];

		public void CopyFrom(MtnCtrlElement src)
		{
			Points = src.Points;
			Angle = src.Angle;
			Transparancy = src.Transparancy;
			StyleOfMovement = src.StyleOfMovement;
			StyleOfRotation = src.StyleOfRotation;
			Time = src.Time;
			Route = (src.Route.Clone() as Vector3[]);
		}
	}
}

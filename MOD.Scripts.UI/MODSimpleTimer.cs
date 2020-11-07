using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODSimpleTimer
	{
		public float timeLeft;

		public MODSimpleTimer()
		{
			this.timeLeft = 0;
		}

		public void Start(float duration)
		{
			this.timeLeft = duration;
		}

		public void Update()
		{
			// Update the timer, making sure it doesn't go below 0
			timeLeft = Math.Max(0, timeLeft - Time.deltaTime);
		}

		public bool Finished()
		{
			return this.timeLeft == 0;
		}
	}
}

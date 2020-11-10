using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODSimpleTimer
	{
		public float timeLeft;
		public bool started;

		public MODSimpleTimer()
		{
			this.timeLeft = 0;
			this.started = false;
		}

		/// <summary>
		/// Start a timer with the given duration
		/// </summary>
		/// <param name="duration"></param>
		public void Start(float duration)
		{
			this.timeLeft = duration;
			this.started = true;
		}

		/// <summary>
		/// Causes the timer to count down as time passes. Call exactly once in unity's Update() loop
		/// </summary>
		public void Update()
		{
			// Update the timer, making sure it doesn't go below 0
			if (this.started)
			{
				timeLeft = Math.Max(0, timeLeft - Time.deltaTime);
			}
		}

		/// <summary>
		/// Indicates if timer was previously started and has now expired
		/// </summary>
		/// <returns></returns>
		public bool Finished()
		{
			return this.started && this.timeLeft == 0;
		}

		/// <summary>
		/// Indicates if the timer is counting down
		/// </summary>
		/// <returns></returns>
		public bool Running()
		{
			return this.started && this.timeLeft != 0;
		}

		/// <summary>
		/// Cancels the timer - causes Finished() to return false, and stops timer counting down
		/// </summary>
		public void Cancel()
		{
			this.started = false;
		}
	}
}

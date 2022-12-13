using UnityEngine;

namespace MouseToColliderDetection {
	public class ClickToCollider
	{
		public static RaycastHit WaitForHit(Camera camera = null, int __mouse_button = 0)
		{
			RaycastHit hit = new RaycastHit();
			if (Input.GetMouseButtonDown(__mouse_button))
			{
				if (camera is null)
					camera = Camera.main;

				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
				Physics.Raycast(ray, out hit);
			}
			return hit;
		}
	}
}
using NextMind.Examples.Steps;
using NextMind.NeuroTags;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.NeuroTagGallery
{
	/// <summary>
	/// <see cref="NeuroTagGalleryStep"/> is the base class for the NeuroTag Gallery steps.
	/// </summary>
	public class NeuroTagGalleryStep : AbstractStep
	{
		[Header("NeuroTags")]

		/// <summary>
		/// List of the NeuroTags in the current scene.
		/// </summary>
		[SerializeField]
		private List<NeuroTag> neuroTags = new List<NeuroTag>();

		/// <summary>
		/// Boolean to check if the NeuroTags are enabled or not.
		/// </summary>
		[SerializeField]
		protected bool areNeuroTagsEnabled = false;

		/// <summary>
		/// Animator for the toggle button to activate the NeuroTags.
		/// </summary>
		[SerializeField]
		protected Animator neuroTagsAnimator;

		protected virtual void Awake()
		{
			neuroTagsAnimator.SetBool("Toggled", areNeuroTagsEnabled);
			UpdateNeuroTags();
		}

		public void ToggleNeuroTags()
		{
			areNeuroTagsEnabled = !areNeuroTagsEnabled;
			neuroTagsAnimator.SetBool("Toggled", areNeuroTagsEnabled);
			UpdateNeuroTags();
		}

		private void UpdateNeuroTags()
		{
			neuroTags.ForEach(neuroTag =>
			{
				neuroTag.enabled = areNeuroTagsEnabled;
				for (int rendererIndex = 0; rendererIndex < neuroTag.StimulationRenderers.Length; ++rendererIndex)
				{
					Renderer renderer = neuroTag.StimulationRenderers[rendererIndex].GetComponent<Renderer>();
					if (renderer == null)
					{
						Image image = neuroTag.StimulationRenderers[rendererIndex].GetComponent<Image>();
						if (image == null)
						{
							Debug.LogWarning("No image nor renderer");
						}
						else
						{
							image.material.SetFloat("_Blend", 1f);
						}
					}
					else
					{
						renderer.material.SetFloat("_Blend", 1f);
					}
				}
			});
		}

		private void Update()
		{
			UpdateAnimation();
		}

		protected virtual void UpdateAnimation() { }
	}
}

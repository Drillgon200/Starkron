using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutlineRenderFeature : ScriptableRendererFeature {
	OutlineRenderPass renderPass;
	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
		if (renderingData.cameraData.cameraType == CameraType.Game) {
			renderer.EnqueuePass(renderPass);
		}
	}
	public override void Create() {
		renderPass = new OutlineRenderPass();
	}
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class OutlineRenderPass : ScriptableRenderPass {
	public OutlineRenderPass() {
		renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
	}
	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
		if (PlayerController.instance != null) {
			CommandBuffer cmd = CommandBufferPool.Get();
			GameManager.instance.DrawOutlines(cmd);
			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();

			CommandBufferPool.Release(cmd);
		}
	}
}

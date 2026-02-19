using UnityEngine;

public class EnemyGroundAttackTriggerHandler : MonoBehaviour {
	public EnemyGround groundEnemy;
	public void BugAttackTrigger() {
		groundEnemy.OnAttackAnimationEvent();
	}
}

using UnityEngine;

public class EnemyTankAnimationEventHandler : MonoBehaviour {
	public EnemyTank tank;
	public void TriggerAttack() {
		tank.TriggerAttack();
	}

	public void FinishAttack() {
		tank.FinishAttack();
	}
}

using UnityEngine;

public interface IDamageable {
    enum DamageSource {
        PLAYER,
        BUG,
        TURRET,
        OTHER
    };
    public void TakeDamage(float amount, Vector3 pos, DamageSource source);
}

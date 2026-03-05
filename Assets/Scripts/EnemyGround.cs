using UnityEngine;

public class EnemyGround : MonoBehaviour, IDamageable, IEnemy {
    public Rigidbody rigidBody;
    public Material bugMaterial;
    public float maxHealth = 20.0F;
    public float turnSpeed = 400.0F;
    public float walkSpeed = 2.0F;
    public float walkDrag = 0.99F;
    public float attackCooldown = 1.0F;
    public float attackCooldownTimer;
    public float damageAmount = 20.0F;
    float health;
    public int attackTriggerFrames;
    public GameObject target;
    public int gameManagerRegisteredIdx;

    public GameObject deathParts;

    enum AnimationState {
        WALKING,
        ATTACKING,
        ATTACK_TRIGGERED
    };
    AnimationState animState = AnimationState.WALKING;
    public float lastAnimOffset;
    public float lastAnimLength;
    public float lastAnimTime;
    public float animOffset;
    public float animLength;
    public float animTime;
    public float animBlendFactor;
    public const float animWalkOffset = 0.0F;
    public const float animWalkLength = 26.0F / 24.0F;
    public const float animAttackOffset = 27.0F;
    public const float animAttackTriggerTime = 4.0F / 24.0F;
    public const float animAttackLength = 17.0F / 24.0F;
    public const float blendTime = 0.15F;
    //ROSS ADDED
    public AudioSource hurtSFX;
    public AudioSource walkSFX;

    void Start() {
        if (GameManager.instance.bugCount >= GameManager.instance.bugCap) {
            // Too many bugs
            Destroy(gameObject);
            return;
        }
        health = maxHealth;
        GameManager.instance.bugCount++;
        gameManagerRegisteredIdx = GameManager.instance.RegisterGroundBug(this);

        SwitchAnimStateTo(AnimationState.WALKING, animWalkOffset, animWalkLength);
        animBlendFactor = 1.0F;
        walkSFX.Play();
    }
    void OnDestroy() {
        GameManager.instance.bugCount--;
        GameManager.instance.RemoveGroundBug(gameManagerRegisteredIdx);
    }

    void SwitchAnimStateTo(AnimationState state, float newOffset, float newLength) {
        lastAnimOffset = animOffset;
        lastAnimLength = animLength;
        lastAnimTime = animTime;
        animOffset = newOffset;
        animLength = newLength;
        animTime = 0.0F;
        animBlendFactor = 0.0F;
        animState = state;
    }

    void Update() {
        float dt = Time.deltaTime;
        animBlendFactor = Mathf.Min(1.0F, animBlendFactor + dt / blendTime);
        lastAnimTime += dt;
        animTime += dt;
        switch (animState) {
            case AnimationState.WALKING: {
                }
                break;
            case AnimationState.ATTACKING: {
                    if (animTime >= animAttackTriggerTime) {
                        attackTriggerFrames = 2;
                        animState = AnimationState.ATTACK_TRIGGERED;
                        goto case AnimationState.ATTACK_TRIGGERED;
                    }
                }
                break;
            case AnimationState.ATTACK_TRIGGERED: {
                    if (animTime >= animAttackLength - blendTime) {
                        SwitchAnimStateTo(AnimationState.WALKING, animWalkOffset, animWalkLength);
                    }
                }
                break;
        }
    }

    void FixedUpdate() {
        float dt = Time.fixedDeltaTime;
        if (animState == AnimationState.WALKING) {
            Vector2 direction =
                target ? Vector2.Normalize(new Vector2(target.transform.position.x - transform.position.x, target.transform.position.z - transform.position.z)) :
                GameManager.instance.GetDirectionToCity(new Vector2(transform.position.x, transform.position.z));
            // Not sure why unity doesn't have a cross product function for 2D vectors built in
            // cross tells us which way to turn, dot slows the turn rate down as it approaches the target. We don't need to be too precise with this.
            float cross = direction.y * transform.forward.x - direction.x * transform.forward.z;
            float dot = Vector3.Dot(transform.forward, new Vector3(direction.x, 0.0F, direction.y));
            rigidBody.MoveRotation(Quaternion.AngleAxis((1.0F - Mathf.Clamp01(dot)) * -Mathf.Sign(cross) * turnSpeed * dt, new Vector3(0.0F, 1.0F, 0.0F)) * rigidBody.rotation);
            if (Vector3.Dot(rigidBody.linearVelocity, transform.forward) < walkSpeed) {
                rigidBody.AddForce(transform.forward * 100.0F, ForceMode.Acceleration);
            }
        }
        if (target != null) {
            bool closeEnoughTarget = (target.transform.position - transform.position).sqrMagnitude < 2.0F * 2.0F;
            if (attackTriggerFrames > 0 && closeEnoughTarget) {
                foreach (Collider collider in Physics.OverlapBox(transform.position + new Vector3(0.0F, 0.532F, 0.0F) + transform.forward * 1.5F, new Vector3(0.5F, 0.5F, 1.0F), transform.rotation)) {
                    PlayerCollisionController player = collider.GetComponent<PlayerCollisionController>();
                    IBugTarget bugTarget = collider.GetComponent<IBugTarget>();
                    if (bugTarget != null || player != null) {
                        bugTarget?.TakeDamage(damageAmount, collider.transform.position, IDamageable.DamageSource.BUG);
                        if (player) {
                            PlayerController.instance.TakeDamage(damageAmount);
                        }
                        attackTriggerFrames = 0;
                        break;
                    }
                }
            }
            if (attackCooldownTimer <= 0.0F && closeEnoughTarget) {
                SwitchAnimStateTo(AnimationState.ATTACKING, animAttackOffset, animAttackLength);
                attackCooldownTimer = attackCooldown;
            }
        }
        rigidBody.linearVelocity *= Mathf.Exp(dt * Mathf.Log(1.0F - walkDrag));
        attackCooldownTimer -= dt;
        attackTriggerFrames = Mathf.Max(attackTriggerFrames - 1, 0);
    }

    public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
        health -= amount;
        //hurtSFX.Play();
        if (health <= 0.0F) {
            if (source == IDamageable.DamageSource.PLAYER) {
                GameManager.instance.statBugsKilledByPlayer++;
            }
            else if (source == IDamageable.DamageSource.TURRET) {
                GameManager.instance.statBugsKilledByTurrets++;
            }
            GameObject fragments = Instantiate(deathParts, transform.position - transform.forward * 1.0F, transform.rotation);
            Destroy(gameObject);
        }
    }
}

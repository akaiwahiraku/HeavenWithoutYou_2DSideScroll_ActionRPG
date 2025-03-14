using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{

    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone Attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    public bool cloneAttackUnlocked { get; private set; }
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private float cloneAttackInBlackhole;
    [SerializeField] private bool canAttack;

    [Header("Aggressive Mirage")]
    [SerializeField] private UI_SkillTreeSlot aggressiveCloneUnlockButton;
    [SerializeField] private float aggressiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("Echoes")]
    [SerializeField] private UI_SkillTreeSlot echoesUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggressiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggressiveClone);
        echoesUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockEchoes);
    }

    #region Unlock region

    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            cloneAttackUnlocked = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAggressiveClone()
    {
        if (aggressiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggressiveCloneAttackMultiplier;
        }
    }

    private void UnlockEchoes()
    {
        if (echoesUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
        }
    }
    #endregion

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggressiveClone();
        UnlockEchoes();
    }

    public void CloneOnAttack(bool followPlayer, bool faceClosestEnemy)
    {
        // クローン生成時に followPlayer と faceClosestEnemy を渡す
        Vector3 cloneAttackOffset = new Vector3(1.0f * player.facingDir, 0, 0);
        SkillManager.instance.clone.CreateClone(player.transform, cloneAttackOffset, followPlayer, faceClosestEnemy);
    }

    public void CreateClone(Transform _clonePosition, Vector3 _offset, bool followPlayerFlag, bool faceClosestEnemy)
    {
        GameObject newClone = Instantiate(clonePrefab);

        // クローン生成時に followPlayerFlag と faceClosestEnemy を渡す
        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, canDuplicateClone, chanceToDuplicate, player, attackMultiplier, followPlayerFlag, faceClosestEnemy);
    }


    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCoroutine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCoroutine(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.2f);
        CreateClone(_transform, _offset, true, true);
    }

}
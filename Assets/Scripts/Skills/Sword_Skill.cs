//using UnityEngine;
//using UnityEngine.UI;


//public enum SwordType
//{
//    //Regular,
//    //Bounce,
//    //Pierce,
//    //Spin
//}

//public class Sword_Skill : SkillData
//{
//   // public SwordType swordType = SwordType.Regular;

//   // [Header("Bounce info")]
//   // [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
//   // [SerializeField] private int bounceAmount;
//   // [SerializeField] private float bounceGravity;
//   // [SerializeField] private float bounceSpeed;

//   // [Header("Pierce info")]
//   // [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
//   // [SerializeField] private int pierceAmount;
//   // [SerializeField] private float pierceGravity;
//   // [SerializeField] private Vector2 pierceLaunchForce;
//   // [SerializeField] private Vector3 pierceLaunchPosition;

//   // [Header("Spin info")]
//   // [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
//   // [SerializeField] private float hitCooldown = .35f;
//   // [SerializeField] private float maxTravelDistance = 3;
//   // [SerializeField] private float spinDuration = 2;
//   // [SerializeField] private float spinGravity = 1;
//   // [SerializeField] private Vector2 spinLaunchForce;
//   // [SerializeField] private Vector3 spinLaunchPosition;

//   //[Header("Skill info")]
//   // [SerializeField] private UI_SkillTreeSlot swordUnlockButton;

//   // public bool swordUnlocked { get; private set; }
//   // [SerializeField] private GameObject swordPrefab;
//   // [SerializeField] private Vector2 launchForce;
//   // [SerializeField] private Vector3 launchPosition;
//   // [SerializeField] private float swordGravity = 0;
//   // [SerializeField] private float freezeTimeDuration;
//   // [SerializeField] private float returnSpeed;

//   // [Header("Passive skills")]
//   // [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
//   // public bool timeStopUnlocked { get; private set; }
//   // [SerializeField] private UI_SkillTreeSlot vulnerableUnlockButton;
//   // public bool vulnerableUnlocked { get; private set; }

//   // private Vector2 finalDir;

//    protected override void Start()
//    {
//        base.Start();

//        //SetupGravity();

//        //swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
//        //bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
//        //pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
//        //spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
//        //timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
//        //vulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulnerable);
//    }

//    //private void SetupGravity()
//    //{
//    //    if (swordType == SwordType.Bounce)
//    //        swordGravity = bounceGravity;
//    //    else if (swordType == SwordType.Pierce)
//    //        swordGravity = pierceGravity;
//    //    else if (swordType == SwordType.Spin)
//    //        swordGravity = spinGravity;
//    //}

//    //protected override void Update()
//    //{
//    //    if (swordType == SwordType.Spin)
//    //        launchForce = spinLaunchForce;
//    //    else if (swordType == SwordType.Pierce)
//    //        launchForce = pierceLaunchForce;

//    //    if (Input.GetKeyDown("joystick button 3"))
//    //        finalDir = new Vector2(player.facingDir * launchForce.x, 0);
//    //}

//    //protected override void CheckUnlock()
//    //{
//    //    UnlockSword();
//    //    UnlockBounceSword();
//    //    UnlockSpinSword();
//    //    UnlockPierceSword();
//    //    UnlockTimeStop();
//    //    UnlockVulnerable();
//    //}

//    //public void CreateSword()
//    //{
//    //    if (swordType == SwordType.Spin)
//    //        launchPosition = spinLaunchPosition;
//    //    else if (swordType == SwordType.Pierce)
//    //        launchPosition = pierceLaunchPosition;
//    //    Vector3 spawnPosition = player.transform.position + player.facingDir * launchPosition;

//    //    GameObject newSword = Instantiate(swordPrefab, spawnPosition, transform.rotation);
//    //    Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

//    //    if (swordType == SwordType.Bounce)
//    //        newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
//    //    else if (swordType == SwordType.Pierce)
//    //        newSwordScript.SetupPierce(pierceAmount);
//    //    else if (swordType == SwordType.Spin)
//    //        newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);


//    //    newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

//    //    player.AssignNewSword(newSword);
//    //}

//    #region Unlock region

//    //private void UnlockTimeStop()
//    //{
//    //    if (timeStopUnlockButton.unlocked)
//    //        timeStopUnlocked = true;
//    //}

//    //private void UnlockVulnerable()
//    //{
//    //    if (vulnerableUnlockButton.unlocked)
//    //        vulnerableUnlocked = true;
//    //}

//    //private void UnlockSword()
//    //{
//    //    if (swordUnlockButton.unlocked)
//    //    {
//    //        swordType = SwordType.Regular;
//    //        swordUnlocked = true;
//    //    }
//    //}

//    //private void UnlockBounceSword()
//    //{
//    //    if (bounceUnlockButton.unlocked)
//    //        swordType = SwordType.Bounce;
//    //}

//    //private void UnlockPierceSword()
//    //{
//    //    if (pierceUnlockButton.unlocked)
//    //        swordType = SwordType.Pierce;
//    //}

//    //private void UnlockSpinSword()
//    //{
//    //    if (spinUnlockButton.unlocked)
//    //        swordType = SwordType.Spin;

//    //    launchForce = spinLaunchForce;
//    //}

//    #endregion


//}

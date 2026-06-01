using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class UISkillSlot : UIBase
{
    [SerializeField] private Image Image_Cooldown;
    [SerializeField] public Image Image_Skill;

    public float CoolTime {  get; set; }
    private float _currentCoolTime;
    private bool _isCooldownActive = false;

    private void Awake()
    {
        // 게임 시작할 때는 쿨타임 이미지를 꺼두거나 투명하게 만듭니다.
        if (Image_Cooldown != null)
        {
            Image_Cooldown.fillAmount = 0f;
        }
    }
    public void OnEnable()
    {
        StageManager.Inst.OnSkillUse += StartCooldown;
    }
    public void StartCooldown()
    {
        _currentCoolTime = CoolTime;
        _isCooldownActive = true;
    }
    private void Update()
    {
        if (!_isCooldownActive) return;

        _currentCoolTime -= Time.deltaTime;

        if (_currentCoolTime <= 0f)
        {
            _currentCoolTime = 0f;
            Image_Cooldown.fillAmount = 0f;
            _isCooldownActive = false;
            Debug.Log(" UI 슬롯: 쿨타임 완료 연출 끝!");
        }
        else
        {
            Image_Cooldown.fillAmount = _currentCoolTime / CoolTime;
        }
    }
}

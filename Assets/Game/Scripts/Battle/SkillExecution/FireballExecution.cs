using Engine;
using Sirenix.OdinInspector;

[Skill("������", 3, typeof(FireballExecution))]
public class SkillConfig_Fireball : SkillConfigBase
{
    [LabelText("�����ٶ�")]
    public float ProjectileSpeed = 10f;

    [LabelText("��ը��Χ")]
    public float ExplosionRadius = 3f;
}

public class FireballExecution : SkillExcution
{
    
    public override void ExecuteUpdate(float time)
    {
        // ������ר���߼������й켣+��ײ���
    }
}

using Engine;
using Sirenix.OdinInspector;

[Skill("»ğÇòÊõ", 3, typeof(FireballExecution))]
public class SkillConfig_Fireball : SkillConfigBase
{
    [LabelText("·ÉĞĞËÙ¶È")]
    public float ProjectileSpeed = 10f;

    [LabelText("±¬Õ¨·¶Î§")]
    public float ExplosionRadius = 3f;
}

public class FireballExecution : SkillExcution
{
    
    public override void ExecuteUpdate(float time)
    {
        // »ğÇòÊõ×¨ÊôÂß¼­£º·ÉĞĞ¹ì¼£+Åö×²¼ì²â
    }
}

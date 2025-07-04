using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Engine
{

    [CreateAssetMenu(fileName = "Skill_Unknow", menuName = "战斗引擎/技能配置")]
    [ConfigType("Skill")]
    public class SkillDataObject : CustomSerializedScriptableObject
    {
        [LabelText("技能描述"), TextArea]
        public string Description = "技能描述未设置";
        [LabelText("初始冷却时间")]
        public float AwakeColdTime;
        [LabelText("冷却时间")]
        public float ColdTime;
        [LabelText("技能检测范围")]
        public float SkillCheckRange;



        [HideLabel, OnValueChanged("CreateSkillConfig"), ValueDropdown("SkillTypeSelect")]
        public string SkillTypeName = "(选择技能模板)";

        private IEnumerable<string> SkillTypeSelect()
        {
            var types = typeof(SkillConfigBase).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => typeof(SkillConfigBase).IsAssignableFrom(x))
                .Where(x => x.GetCustomAttribute<SkillAttribute>() != null)
                .OrderBy(x => x.GetCustomAttribute<SkillAttribute>().Order)
                .Select(x => x.GetCustomAttribute<SkillAttribute>().Type);
            var results = types.ToList();
            return results;
        }
        private void CreateSkillConfig()
        {
            if (SkillTypeName != "(选择技能模板)")
            {
                var skillType = typeof(SkillConfigBase).Assembly.GetTypes()
                    .Where(x => !x.IsAbstract)
                    .Where(x => typeof(SkillConfigBase).IsAssignableFrom(x))
                    .Where(x => x.GetCustomAttribute<SkillAttribute>() != null)
                    .Where(x => x.GetCustomAttribute<SkillAttribute>().Type == SkillTypeName)
                    .First();
                if (SkillTemplateConfig != null && SkillTemplateConfig.GetType() == skillType) return;
                SkillTemplateConfig = Activator.CreateInstance(skillType) as SkillConfigBase;
            }
        }

        public bool IsShowSkillConfig => SkillTypeName != "(选择技能模板)";
        [LabelText("$SkillTypeName"), ShowIf("IsShowSkillConfig"), HideReferenceObjectPicker]
        public SkillConfigBase SkillTemplateConfig;

    }
}
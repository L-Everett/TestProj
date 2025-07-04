using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class SkillAttribute : Attribute
    {
        public SkillAttribute(string type, int order, Type excutionType)
        {
            Type = type;
            Order = order;
            ExcutionType = excutionType;
        }
        public string Type { private set; get; }
        public int Order { private set; get; }
        public Type ExcutionType { private set; get; }
    }
    public abstract class SkillConfigBase
    {
        public string Name = "";
        public virtual string InfoLabel { get; set; } = "模版说明";
        protected virtual bool IsFixedTargetSelect { get { return false; } }
    }
   
}

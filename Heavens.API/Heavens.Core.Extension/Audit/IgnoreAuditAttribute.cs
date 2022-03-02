using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.Audit;
/// <summary>
/// 忽略操作审计、数据审计
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
public class IgnoreAuditAttribute : Attribute
{
}

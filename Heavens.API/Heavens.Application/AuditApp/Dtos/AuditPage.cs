using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Application.AuditApp.Dtos;
public class AuditPage:AuditDto
{
    public bool HasBody { get; set; }
    public bool HasQuery { get; set; }
    public bool HasException { get; set; }
}

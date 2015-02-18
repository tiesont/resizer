using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    public interface IAuthorizationResult
    {
        bool DenyRequest { get; }

        IEnumerable<string> Reasons { get; }
    }
}

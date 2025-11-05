using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

internal sealed class UserInfo
{
    public string? UserName { get; set; }
    public int? UserAge { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserTask.AddOns.Endpoints.Models
{
    public class WorkfowInstanceUsertaskViewModel
    {
        List<UsertaskViewModel> userTasks { get; set; } = new List<UsertaskViewModel>();
    }
}

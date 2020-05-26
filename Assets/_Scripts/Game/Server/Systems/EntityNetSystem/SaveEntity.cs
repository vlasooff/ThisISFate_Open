using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SaveEntity : SaveBehaviour
{
    public SaveEntity Load()
    {
        return LoadJSON<SaveEntity>(ServerConfig.patchServer);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace OthelloMillenniumClient
{
    interface ILobby
    {
        void OnLaunchGameServer();
        void OnUpdateOpponentColorServer(Color color, int AvatarId);
    }
}

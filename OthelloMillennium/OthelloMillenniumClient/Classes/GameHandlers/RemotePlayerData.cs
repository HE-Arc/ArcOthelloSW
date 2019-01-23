using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    class RemotePlayerData
    {
        public string Name { get; set; }
        public int AvatarId { get; set; }
        public PlayerType PlayerType{ get; set; }
    }
}

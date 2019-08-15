using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Community.Core
{

    public static class NetworkGeneral
    {
        public const int ProtocolId = 1;
          public static readonly int PacketTypesCount = Enum.GetValues(typeof(PacketType)).Length;

        public const int MaxGameSequence = 512;
        public const int HalfMaxGameSequence = MaxGameSequence / 2;

        public static int SeqDiff(int a, int b)
        {
            return Diff(a, b, HalfMaxGameSequence);
        }
        public static int Diff(int a, int b, int halfMax)
        {
            return (a - b + halfMax * 3) % (halfMax * 2) - halfMax;
        }
    }
}

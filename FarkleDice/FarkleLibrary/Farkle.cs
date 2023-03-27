using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.Threading;

namespace FarkleLibrary
{
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void Update(int count, int nextClient, bool gameOver);
    }

    public interface IFarkle
    {
        [OperationContract]
        int JoinGame();
        [OperationContract(IsOneWay = true)]
        void LeaveGame();
        [OperationContract(IsOneWay = true)]
        void NextCount();
    }
}
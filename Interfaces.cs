using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApFrameworkLite
{

    public enum AddtionalRequestActionSequence
    {
        AtStart,
        BeforeRequestExecution,
        AfterRequestExecution,
        AtEnd,
    }

    public interface IRequestProcessor
    {
        Task ProcessRequestAsync(RequestActionDelegate requestActionDelegate, bool continueOnCapturedContext);

        void AddAddtionalAction(RequestActionDelegate additionalRequestAction, bool continueOnCapturedContext, AddtionalRequestActionSequence addtionalActionSequence);
        void AddAddtionalAction(RequestActionDelegate additionalRequestAction, AddtionalRequestActionSequence addtionalActionSequence);

        public object RequestContextBag { get; set; }

    }
}

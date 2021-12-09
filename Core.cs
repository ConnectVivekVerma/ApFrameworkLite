using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApFrameworkLite
{
   

    public class RequestActionWrapper
    {
        public RequestActionWrapper(RequestActionDelegate requestActionDelegate, bool continueOnCapturedContext)
        {
            AdditionalRequestAction = requestActionDelegate;
            ContinueOnCapturedContext = continueOnCapturedContext;
        }

        public RequestActionDelegate AdditionalRequestAction { get; set; }

        public bool ContinueOnCapturedContext { get; set; }
    }

    public class RequestProcessor : IRequestProcessor
    {

        private List<RequestActionWrapper> _atStart;
        private List<RequestActionWrapper> _beforeRequestExecution;        
        private List<RequestActionWrapper> _afterRequestExecution;
        private List<RequestActionWrapper> _atEnd;        

        public RequestProcessor()
        {
            _atStart = new List<RequestActionWrapper>();
            _beforeRequestExecution = new List<RequestActionWrapper>();
            _afterRequestExecution = new List<RequestActionWrapper>();
            _atEnd = new List<RequestActionWrapper>();            
        }

        public object RequestContextBag { get; set; }

        public void AddAddtionalAction(RequestActionDelegate additionalRequestAction, bool continueOnCapturedContext, AddtionalRequestActionSequence addtionalActionSequence)
        {
            switch (addtionalActionSequence)
            {
                case AddtionalRequestActionSequence.AtStart:
                    _atStart.Add(new RequestActionWrapper(additionalRequestAction, continueOnCapturedContext));
                    break;

                case AddtionalRequestActionSequence.AtEnd:
                    _atEnd.Add(new RequestActionWrapper(additionalRequestAction, continueOnCapturedContext));
                    break;

                case AddtionalRequestActionSequence.BeforeRequestExecution:
                    _beforeRequestExecution.Add(new RequestActionWrapper(additionalRequestAction, continueOnCapturedContext));
                    break;

                case AddtionalRequestActionSequence.AfterRequestExecution:
                    _afterRequestExecution.Add(new RequestActionWrapper(additionalRequestAction, continueOnCapturedContext));
                    break;
            }
        }
        public void AddAddtionalAction(RequestActionDelegate additionalRequestAction, AddtionalRequestActionSequence addtionalActionSequence)
        {
            AddAddtionalAction(additionalRequestAction, true, addtionalActionSequence);
        }

        public async Task ProcessRequestAsync(RequestActionDelegate requestActionDelegate, bool continueOnCapturedContext)
        {

            foreach (var request in _atStart)
            {
                await request.AdditionalRequestAction(RequestContextBag).ConfigureAwait(request.ContinueOnCapturedContext);
            }

            foreach (var request in _beforeRequestExecution)
            {
                await request.AdditionalRequestAction(RequestContextBag).ConfigureAwait(request.ContinueOnCapturedContext);
            }

            await requestActionDelegate(RequestContextBag).ConfigureAwait(continueOnCapturedContext);

            foreach (var request in _afterRequestExecution)
            {
                await request.AdditionalRequestAction(RequestContextBag).ConfigureAwait(request.ContinueOnCapturedContext);
            }

            foreach (var request in _atEnd)
            {
                await request.AdditionalRequestAction(RequestContextBag).ConfigureAwait(request.ContinueOnCapturedContext);
            }

        }
    }
}

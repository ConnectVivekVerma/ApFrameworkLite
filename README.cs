# ApFrameworkLite

private async void HowToUse()
        {

            /*
             * Step 1: Get Request Processor   
             *      A few ways to do this 
             *          - Extend RequestProcessor class and add required pre and post methods. Then inject instacne to construstor
             *          - Create RequestProcessorFactory to return RequestProcessor instnace with specific pre/post methods addehttps://github.com/ConnectVivekVerma/ApFrameworkLited based on parameter passed. 
             *          
             * Step 2:
             *      Call ProcessRequestAsync method on processor and pass in actual action to perform
             */

            //get an instance from factory
            /*
             * Lets say, i have a function to - 
             *      get trades from data source (service, DB, etc. ) 
             *      map to entity and return back object
             *      on error return status code back             
             *      
             *  I will wrap call to this in RequestProcessor
             */


            RequestActionDelegate getData = async (bag) =>
            {
                // Get bag
                var dataBag = bag as DataBag;

                // Get trades
                var trades = await DataStore.GetTradesByDate(dataBag.TradeDate);

                // Map
                var tradeJson = await Mapper.Map(trades);

                // return, by setting in bag. Can also set as local variable within function if object is not needed by any other actions in request processor
                dataBag.MappedJson = tradeJson;

            };

            IRequestProcessor requestProcessor = RequestProcessorFactory.GetRequestProcessor("With_Authorisation_ExceptionLogging_PerfromanceMonitoring");

            await requestProcessor.ProcessRequestAsync(getData, false);

        }

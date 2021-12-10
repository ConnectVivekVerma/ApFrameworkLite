using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApFrameworkLite;
using System.Text;
using System.Threading.Tasks;

namespace ApFrameworkLiteTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public async Task ActionsShouldExecuteInSequence()
        {

            //Arrange
            var testBag = new SampleBag() { LogBuilder = new StringBuilder() };

            RequestActionDelegate first = (bag) => AddMsgToBuilder("1", (bag as SampleBag).LogBuilder);
            RequestActionDelegate before = (bag) => AddMsgToBuilder("2", (bag as SampleBag).LogBuilder);
            RequestActionDelegate after = (bag) => AddMsgToBuilder("4", (bag as SampleBag).LogBuilder);
            RequestActionDelegate last = (bag) => AddMsgToBuilder("5", (bag as SampleBag).LogBuilder);

            var reqProcessor = new RequestProcessor();
            reqProcessor.RequestContextBag = testBag;

            reqProcessor.AddAddtionalAction(first, true, AddtionalRequestActionSequence.AtStart);
            reqProcessor.AddAddtionalAction(before, true, AddtionalRequestActionSequence.BeforeRequestExecution);
            reqProcessor.AddAddtionalAction(after, true, AddtionalRequestActionSequence.AfterRequestExecution);
            reqProcessor.AddAddtionalAction(last, true, AddtionalRequestActionSequence.AtEnd);

            //Act
            await reqProcessor.ProcessRequestAsync((bag) => AddMsgToBuilder("3", (bag as SampleBag).LogBuilder), false);

            //Assert
            Assert.AreEqual("12345", testBag.LogBuilder.ToString());

        }

        [DataTestMethod]
        [DataRow(AddtionalRequestActionSequence.AtStart, "START", "1START2345")]
        [DataRow(AddtionalRequestActionSequence.BeforeRequestExecution, "BFR", "12BFR345")]
        [DataRow(AddtionalRequestActionSequence.AfterRequestExecution, "AFTER", "1234AFTER5")]
        [DataRow(AddtionalRequestActionSequence.AtEnd, "END", "12345END")]
        public async Task MultipeActionsAddedToSequnce_ShouldExecuteInAddedOrder(AddtionalRequestActionSequence addtionalRequestActionSequence, string value, string expected)
        {
            var testBag = new SampleBag() { LogBuilder = new StringBuilder() };

            var reqProcessor = new RequestProcessor();
            reqProcessor.RequestContextBag = testBag;

            reqProcessor.AddAddtionalAction((bag) => AddMsgToBuilder("1", (bag as SampleBag).LogBuilder), true, AddtionalRequestActionSequence.AtStart);
            reqProcessor.AddAddtionalAction((bag) => AddMsgToBuilder("2", (bag as SampleBag).LogBuilder), true, AddtionalRequestActionSequence.BeforeRequestExecution);
            reqProcessor.AddAddtionalAction((bag) => AddMsgToBuilder("4", (bag as SampleBag).LogBuilder), true, AddtionalRequestActionSequence.AfterRequestExecution);
            reqProcessor.AddAddtionalAction((bag) => AddMsgToBuilder("5", (bag as SampleBag).LogBuilder), true, AddtionalRequestActionSequence.AtEnd);

            reqProcessor.AddAddtionalAction((bag) => AddMsgToBuilder(value, (bag as SampleBag).LogBuilder), true, addtionalRequestActionSequence);

            await reqProcessor.ProcessRequestAsync((bag) => AddMsgToBuilder("3", (bag as SampleBag).LogBuilder), false);
            
            
            //Assert
            Assert.AreEqual(expected, testBag.LogBuilder.ToString());

        }

        private async Task AddMsgToBuilder(string s, StringBuilder sb)
        {
            sb.Append(s);
            await Task.CompletedTask;
        }

        private class SampleBag
        {
            public StringBuilder LogBuilder { get; set; }
        }

    }



}

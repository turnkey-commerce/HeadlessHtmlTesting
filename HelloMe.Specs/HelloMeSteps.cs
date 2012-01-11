using TechTalk.SpecFlow;
using HeadlessHtmlTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HtmlAgilityPack;
using System.Xml;

namespace HelloMe.Specs {
    [Binding]
    public class HelloMeSteps {

        private HeadlessHtmlSession _htmlSession;
        private string _response;
        private string _url = "http://localhost:56966/Hello/Create";

        [Given(@"I am on the HelloMe web page")]
        public void GivenIAmOnTheHelloMeWebPage() {
            _htmlSession = new HeadlessHtmlSession();
            _htmlSession.Get(_url);
            Assert.IsTrue(_htmlSession.HtmlDoc.DocumentNode.SelectSingleNode("//h2").InnerText.Equals("Hello Me"));
            
        }

        [Given(@"I have entered my name into the Name field")]
        public void GivenIHaveEnteredMyNameIntoTheNameField() {
            _htmlSession.FormElements["name"] = "Joe Blow";
            // Just checks that the element is present. The form elements will be assembled at time of the post.
            Assert.IsNotNull(_htmlSession.HtmlDoc.DocumentNode.SelectSingleNode("//input[@type='text' and @name='name']").Attributes);
        }

        [When(@"I submit the form")]
        public void WhenISubmitTheForm() {
            _htmlSession.Post(_url);
        }

        [Then(@"the browser should say ""Hello"" to my name")]
        public void ThenTheBrowserShouldSayHelloToMyName() {
            Assert.IsTrue(_htmlSession.HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='greeting']").InnerText.Equals("Hello Joe Blow!"));
        }

    }
}

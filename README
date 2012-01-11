Headless HTML Testing
---------------------

This project is an example of using the [HTML Agility Pack](http://htmlagilitypack.codeplex.com/) to enable "headless browser" HTML tests for
BDD or Integration tests. This particular project uses the [SpecFlow](http://specflow.org/) package to enable the tests.  Then the tests use the HeadlessHtmlTesting
library to call the website and parse the results.  The library is based on the post by Rohit Agarwal - [Using HtmlAgilityPack to GET and POST web forms](http://refactoringaspnet.blogspot.com/2010/04/using-htmlagilitypack-to-get-and-post.html).

Generally the tests run much faster than tests that actually bring up a browser such as Watin or Selenium, and are more suitable for testing betwen 
builds and TDD/BDD approaches.

The example is a simple website that presents a form for the user to type their name and the website responds with a "Hello Name" to the user. The tests 
check for the appropriate content using the "XPath" method of parsing the HTML. The test project is set up to automatically run IIS Express to start the
website when the tests are run.
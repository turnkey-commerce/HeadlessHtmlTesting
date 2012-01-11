Feature: Hello to Me
	In order to boost my esteem
	As a user
	I want the computer to say hello to me

Scenario: Hello to me
	Given I am on the HelloMe web page
	And I have entered my name into the Name field
	When I submit the form
	Then the browser should say "Hello" to my name
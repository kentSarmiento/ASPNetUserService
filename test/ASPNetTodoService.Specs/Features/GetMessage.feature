Feature: Get Message
  As a User,
  I want to get a message from authorization server
  So that I can verify if I am already authenticated/authorized

Background:

  Given there are no users registered in the system

Scenario: Get message as logged in user

  Given user is registered in the system
    | Email         | Password      |
    | user@mail.com | P@ssw0rd1234  |

  And user is logged in the system
    | Email         | Password      |
    | user@mail.com | P@ssw0rd1234  |

  When user retrieves message from the system
  Then message is retrieved containing user name

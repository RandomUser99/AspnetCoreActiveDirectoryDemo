# AspnetCoreActiveDirectoryDemo

A quick and dirty demo application on how to work with Active Directory using ASP.Net Core using Feature folders and MediatR.

The front end/MVC views aren't wired up to the controller. It's just back end code to show how I would go about creating a new user. To add that functionality, just add a new form to the Features\Users\Create.cshtml using ActiveDirectoryDemo.Features.Users.Create.Command as the model for the form. 

For more info on Feature folders (vertical slice architecture) see these:

https://jimmybogard.com/vertical-slice-architecture/

https://github.com/jbogard/ContosoUniversityDotNetCore

For more info on MediatR see this:

https://github.com/jbogard/MediatR

# MSStackExample
Reference application to demo simple but functional DDD using WebAPI, EF, and SignalR.
Namespace was originally ESTest as I started to intend to demo Event Sourcing, but got distracted with SignalR and DDD. 
The only DDD concepts present here are rich domain entities (as opposed to anemic) contexts, and repos. 
Context Maps, Domain Services, and Anti-Corruption Layers are not yet part of the solution and may not be 
needed without creating a need for demo purposes.

Future plans are to implement js unit testing and build out remaining functionality/demo concepts.

## Prerequisites to local development: 
* Visual Studio 2015/2017 (perferrably 2017 with all of the latest updates)
* SQL Server Developer 2014 (or greater, preferrably 2017) with local admin rights with a default instance of localhost
  * Note: You should be able to connect via windows credentials from SQL Server Management Studio to localhost (not the machine name, not localdb) and have complete sa-level access.

## To run/debug: 
* Clone repo 
* Build Solution
* Set startup project as ESTest.Api
* Open Package Manager Console
* Set "Default project" dropdown to data\ESTest.DAL
* Run the following command in the Package Manager Console: `Update-Database`
* Go to the Solution properties window (right-click at the solution root and click Properties)
* Under Common Properties/Startup Project choose Multiple startup projects
* Under the Action column of each project, ensure they are all set to None, however for ESTest and ESTest.Api, ensure they are set to Start
* Go to the properties screen of the ESTest.Api project (right-click on the project root and click Properties)
* On the Web section, under the Start Action section, ensure "Don't open a page. Wait for a request from an external application." is selected
* Go to the properties screen of the ESTest project (right-click on the project root and click Properties)
* On the Web section, under the Start Action section, ensure "Specific Page" is selected and ensure the value in the textbox next to this is blank
* On the front end web project properties, ensure that the Project URL value matches the SSL URL for that project (left click on the project and observe the properties window below the solution explorer to see this value)
  * Currently for the ESTest project, this value should be https://localhost:44342/
* Build solution and launch debugging!

The EF migration will have seeded a couple of default users in the system. See the data/ESTest.DAL/Migrations/Configuration.cs to see these logins and passwords to test the system with.
To see the full effect of the demo, it is recommended that you login with one of the pair available from the web browser (e.g. Chrome) that was launched from debugging, and then launching a different browser (e.g. Firefox) to navigate to the same local site and logging in with the other login. Then navigate in each browser to the Live Comment Board using the link at the top of the home page.
Currently there is not a way to add a new chat room, but a default one called "Test Chat Room" will have been seeded.


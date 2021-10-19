What is Dependency Injection?


class ---->(uses) Other class (This class becomes a dependency)

public ItemsController()
{
  repository=new InMemItemsRepository();
}

Flip things so that they become like this

public ItemsController(repository)
{
  this.repository=repository;
}

In this case we are injecting repository dependency

Dependency Inversion Principle


Class --depends on --> Dependency A
The above is not idea 
Class depends on  an abstraction (Interface)  and the actual dependency implement or satisify the contract which is the Interface

Class                          Dependency A
     \                        /
      \Depends on            / Implements
       \                    /
        \                  /
         \____Interface___/

Class is happy to work with any dependency that satisifies the contract and does not worry with how the contract is satisified


Why ?
--> By having our code depend upon abstractions we are decoupling implementations from each other 
-->Code is cleaner ,easier to modify and easier to reuse
How to construct the dependencies?

Dependency A
Dependency B   ----> IServiceProvider     Class
Dependency C         Service Container

App Startup each of these dependecies are registered into the service container
When the class gets instantiated then the Service Container will take care of the following Resolve,construct and inject dependencies


Data Transfer Object DTO

Is the contract that will be established between the client and service


Data annotation 
Items we can add to props to say that they are required or not

examples here
[Required] or even [Range]


Asynchronous programming

 Waiting for processes that are not dependent on each other like making a breakfast instead

Do different things seperately that dont have to use a result from another process
You go back to finish it up when there is a result to move to the next stage


we want to make calls to the database which are expensive operations to be asynchronous so that callers do not wait but continue and receive the result later
Async all the way



Page with healthchecks for AspNetCore


https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
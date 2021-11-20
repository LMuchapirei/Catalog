using System;
using Xunit;
using Catalog.Api.Repositories;
using Moq;
using Catalog.Api.Entities;
using Microsoft.Extensions.Logging;
using Catalog.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Catalog.Api.Dtos;
using FluentAssertions;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub=new();
        private readonly Mock<ILogger<ItemsController>> loggerStub=new ();
        private readonly Random random=new();
        [Fact]  // Tell that this is a method of the class that needs to be tested
        //define the methods as follows
        // public returnType UnitOfWork_StateUnderTest_ExpectedBehavior()  UnitOfWork being the function we are testing, Conditions while we are testing, What we expect to get after testing
        public async Task GetItemAsync_WithNonExistingItem_ReturnsNotFound()
        {
            // Arrange  -->setup before executing the test mocking,imports etc
            repositoryStub.Setup(repo=>repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Item)null);

            var loggerStub=new Mock<ILogger<ItemsController>>();

            var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);
            // Act  --> Actual execution of the test

            var result= await controller.GetItemAsync(Guid.NewGuid());

            // Assert  --> Verify whatever needs to be verified for the execution results
            // Assert.IsType<NotFoundResult>(result.Result);
            result.Result.Should().BeOfType<NotFoundResult>();
            return;
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            // Arrange
            var expectedItem=CreateRandomItem();

            repositoryStub.Setup(repo=>repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedItem);

            var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);

            // Act
            var result=await controller.GetItemAsync(Guid.NewGuid());
            // Assert
            // Assert.IsType<ItemDto>(result.Value);
            // var dto=(result as ActionResult<ItemDto>).Value;
            // Assert.Equal(expectedItem.Id,dto.Id);
            // Assert.Equal(expectedItem.Name,dto.Name);
            // Assert.Equal(expectedItem.CreatedDate,dto.CreatedDate);
            // Assert.Equal(expectedItem.Price,dto.Price);  this is not okay don't need this many asserstions okay
            result.Value.Should().BeEquivalentTo(expectedItem,
            options=>options.ComparingByMembers<Item>());  // focus on the properties only and match over each
        }

                [Fact]
        public async Task GetItemsAsync_WithExistingItem_ReturnsAllItems()
        {
            //Arrange
            var expectedItems=new []{CreateRandomItem(),CreateRandomItem(),CreateRandomItem()};

            repositoryStub.Setup(repo=>repo.GetItemsAsync())
                    .ReturnsAsync(expectedItems);

            var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);
            

            //Act
            var actualItems=await controller.GetItemsAsync();

            //Assert
            actualItems.Should().BeEquivalentTo(
                expectedItems,
                MvcOptions=>MvcOptions.ComparingByMembers<Item>()
            );
        }

        [Fact]
        
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            // Arrange
            var itemToCreate=new CreateItemDto(){
                Name=Guid.NewGuid().ToString(),
                Price=random.Next(1000)
            };
            var controller=new ItemsController(repositoryStub.Object,loggerStub.Object);

            //Act
            var result= await controller.CreateItemAsync(itemToCreate);
            
            // Assert
            var createdItem=(result.Result as CreatedAtActionResult).Value as ItemDto;
            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                opptions=>opptions.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow,TimeSpan.FromMilliseconds(1000));
        }
        

        private Item CreateRandomItem(){
            return new (){
                Id=Guid.NewGuid(),
                Name=Guid.NewGuid().ToString(),
                Price=random.Next(1000),
                CreatedDate=DateTimeOffset.UtcNow
            };
        }
    }
}


//Stub fake version of dependencies that are used for the sole purposes of testing
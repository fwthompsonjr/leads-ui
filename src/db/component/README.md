# About
---

legallead.db.connector is a light weight data storage provider that stores entities as JSON. 
This package addresses the need to establish a RDMBS-like storage solution without the 
overhead of a full blown database.

## How to Use
---

This package is build targeting NETCORE projects.  

There are two basic steps needed to get started.  
1. Create a POCO class that inherits `DataEntity<T>`
2. Initialize an instance of the `DataProvider` class to manage objects.
3. The `DataProvider` implements public interface `IDataProvider` this 
is intended to support IoC and DI patterns.

Working with your entities is simple, with the following actions defined:
- **Create** operation uses the Insert method of the DataProvider
- **Read** operation uses  
    - `_FirstOrDefault_` to find first record matching lambda expression
    - `_Where_` to find all records matching lambda expression
- **Update** operation uses the Update method of the `DataProvider`
- **Delete** operation uses the Delete method of the `DataProvider`
- **Create** operation uses the Insert methodof the `DataProvider`

## Usage
---   

1. Define a DataEntity class called FruitEntity this base class includes Id, Name properties and no additional properties will be defined for the example case.
	```C#
	using legallead.json.db.entity;
	namespace legallead.json.db.tests.sample
	{
		// please note that the base class is inherited
		// and the typeref parameter is the same as the class
		// definition that is being created
		public class FruitEntity : DataEntity<FruitEntity>
		{
		}
	}
	```
1. Insert Method
	```C#


	[Fact]
	public void Db_Can_Insert_A_Record()
	{
		var fruit = new FruitEntity
		{
			Name = "Banana"
		};
		Provider.Insert(fruit);
		// note: attempting to inset with Id populated will 
		Assert.NotNull(fruit.Id);
	}

	```
1. Update Method
	```C#
	[Fact]
	public void Db_Can_Update_A_Record()
	{
		const string otherName = "Pear";
		var fruit = new FruitEntity
		{
			Name = "Banana"
		};
		Provider.Insert(fruit);
		// now change the fruit 
		fruit.Name = otherName;
		Provider.Update(fruit);
		// fetch record from db
		var updated = Provider.FirstOrDefault<FruitEntity>(x => x.Id == fruit.Id);
		Assert.NotNull(updated);
		Assert.Equal(otherName, updated.Name);
	}

	```
1. Delete Method
	```C#
	[Fact]
	public void Db_Can_Delete_A_Record()
	{
		var fruit = new FruitEntity
		{
			Name = "Banana"
		};
		Provider.Insert(fruit);
		// now delete the fruit 
		Provider.Delete(fruit);
		// fetch record from db
		var deleted = Provider.FirstOrDefault<FruitEntity>(x => x.Id == fruit.Id);
		Assert.Null(deleted);
	}
	```

## Feedback
---  
We hope that you have as much fun using this package as we had creating it! 
You're feedback is welcome, please reach out to LegalLead Tech Team with your comments.

#### Release Notes:

1.0.0 - 20231115 - Initial package creation
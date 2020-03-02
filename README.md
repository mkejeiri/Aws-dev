
# Aws development samples

### Prior settings  

Need to set up the following:
- create a user and give it the appropriate right (using policies, for dev purposes I used AmazonDynamoDBFullAccess policy)
- download user the credentials (csv file) and use aws configure command to be able to authenticate from your machine (this is use by the DynamoDB client to connect to the cloud)
- go the DynamoDB Dashboard and Create table with the following structure: 
ssh`
{
  "Actors": {
    "SS": [
      "Movie1",
      "Movie2"
    ]
  },
  "Description": {
    "S": "Movie Description"
  },
  "MovieName": {
    "S": "Movie"
  },
  "RankedDateTime": {
    "S": "01/03/2020 16:32:04"
  },
  "Ranking": {
    "N": "1"
  },
  "UserId": {
    "N": "1"
  }
}
`

- Create an index with MovieName partition key
- to avoid any future cost from Amazon adjust Read capacity units/Write capacity units to 1


### Interaction with DynamoDB instance in the cloud 

We used three methods to interact with DynamoDB 
- Low Level Model (see LowLevelModelRepository)
- Persistence Object Model(see PersistenceObjectModelRepository)
- Document Model (see DocumentModelRepository)

In addition :
- Create and delete DynamoDB (see DDLMovieRankRepository)
- The project also include a data seeder.. 

### Integration test and local DynamoDB instance

We use collection fixture class which is an xUnit-specific class that allows us to run code before running the tests, we use it to: 
- run the code needed to spin up in-memory test server
- set up  HttpClient
- create  DynamoDB table ready for test
- create a test context inside collection fixture to pull down a local DynamoDB Docker image, and then run it
- create a test data setup class that will allow to run tests against local DynamoDB table (i.e. switch between using local and costly cloud DynamoDB instance)

The **purpose** here is to spin up and run an **in-memory local instance of dynamoDB** with a docker container, we want also to set up, **in-memory TestServer** and **HttpClient**, This will allow to spin up an in-memory instance of the app and make requests to it. 


- We need to add some code to MovieRank solution startup class to determine if we are running application locally or in a production-like environment and 
- then we create tests scenarios that will test each of the created endpoints in the Working With Items in DynamoDB using the Object Persistence model

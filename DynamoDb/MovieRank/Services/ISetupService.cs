using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRank.Services
{
    public interface ISetupService
    {
        Task CreateDynamoDbTable(string dynamoDbTableName);

        Task DeleteDynamoDbTable(string dynamoDbTableName);
    }
}

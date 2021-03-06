﻿using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ThoughtHaven.Azure.Storage.Table;

namespace ThoughtHaven.AspNetCore.Identity.AzureTableStorage.Fakes
{
    public class FakeTableEntityStore : TableEntityStore
    {
        new public FakeTableExistenceTester ExistenceTester =>
            (FakeTableExistenceTester)base.ExistenceTester;
        new public FakeCloudTable Table => (FakeCloudTable)base.Table;

        public FakeTableEntityStore()
            : base(new FakeCloudTable("Fake"), new FakeTableExistenceTester(),
                  new TableRequestOptions())
        { }

        public string Retrieve_InputPartitionKey;
        public string Retrieve_InputRowKey;
        public DynamicTableEntity Retrieve_Output = new DynamicTableEntity()
        { PartitionKey = "pk", RowKey = "rk" };
        public override Task<DynamicTableEntity> Retrieve(string partitionKey, string rowKey)
        {
            this.Retrieve_InputPartitionKey = partitionKey;
            this.Retrieve_InputRowKey = rowKey;

            return Task.FromResult(this.Retrieve_Output);
        }

        public ITableEntity Insert_InputEntity;
        public override Task Insert<TEntity>(TEntity entity)
        {
            this.Insert_InputEntity = entity;

            return Task.CompletedTask;
        }

        public ITableEntity Replace_InputEntity;
        public override Task Replace<TEntity>(TEntity entity)
        {
            this.Replace_InputEntity = entity;

            return Task.CompletedTask;
        }

        public ITableEntity Delete_InputEntity;
        public StorageException Delete_ExceptionToThrow = null;
        public override Task Delete<TEntity>(TEntity entity)
        {
            this.Delete_InputEntity = entity;

            if (this.Delete_ExceptionToThrow != null) { throw this.Delete_ExceptionToThrow; }

            return Task.CompletedTask;
        }
    }
}
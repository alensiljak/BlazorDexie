﻿using BlazorDexie.JsModule;
using BlazorDexie.Test.Database;
using BlazorDexie.Test.TestItems;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlazorDexie.Test
{
    public class CollectionTest
    {
        private IModuleFactory _moduleFactory;

        public CollectionTest(IModuleFactory moduleFactory)
        {
            _moduleFactory = moduleFactory;
        }

        [Fact]
        public async Task Count()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            int count = await db.TestItems.Count();

            // assert
            Assert.Equal(4, count);
        }

        [Fact]
        public async Task Filter()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Filter("return i.name === p[0]", new[] { "CC" }).ToArray();

            // assert
            Assert.Single(testItems);
            Assert.Equal(initialItems[2].Id, testItems[0].Id);
        }

        [Fact]
        public async Task FilterModule()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.FilterModule("../../../DexieWrapper.Test/wwwroot/scripts/customFilter.mjs", new[] { "CC" }).ToArray();

            // assert
            Assert.Single(testItems);
            Assert.Equal(initialItems[2].Id, testItems[0].Id);
        }

        [Fact]
        public async Task OffsetLimit()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.OrderBy(nameof(TestItem.Name)).Offset(1).Limit(2).ToArray();

            // assert

            Assert.Equal(new[] { "BB", "CC" }, testItems.Select(t => t.Name).ToList());
        }

        [Fact]
        public async Task Reverse()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]{
                new TestItem() { Id = Guid.NewGuid(), Name = "C" },
                new TestItem() { Id = Guid.NewGuid(), Name = "A" },
                new TestItem() { Id = Guid.NewGuid(), Name = "B" }
            };

            var exceptedNames = initialItems.OrderByDescending(i => i.Id).Select(i => i.Name).ToArray();

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Reverse().ToArray();

            // assert
            Assert.Equal(3, testItems.Length);
            Assert.Equal(exceptedNames, testItems.Select(t => t.Name).ToArray());
        }

        [Fact]
        public async Task ToArrayAndToList()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItemArray = await db.TestItems.ToArray();
            var testItemList = await db.TestItems.ToList();

            // assert
            Assert.Equal(4, testItemArray.Length);
            Assert.Equal(4, testItemList.Count);

            foreach (var initialItem in initialItems)
            {
                Assert.Equal(initialItem.Name, testItemArray.First(i => i.Id == initialItem.Id).Name);
                Assert.Equal(initialItem.Name, testItemList.First(i => i.Id == initialItem.Id).Name);
            }
        }

        private MyDb CreateDb()
        {
            var databaseId = Guid.NewGuid().ToString();
            return new MyDb(_moduleFactory, databaseId);
        }
    }
}

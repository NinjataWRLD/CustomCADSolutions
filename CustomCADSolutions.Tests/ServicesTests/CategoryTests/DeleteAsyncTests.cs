﻿namespace CustomCADSolutions.Tests.ServiceTests.CategoryTests
{
    public class DeleteAsyncTests : BaseCategoriesTests
    {
        [TestCase(1)]
        [TestCase(4)]
        public async Task Test_DeletesWhenCategoryExists(int id)
        {
            await service.DeleteAsync(id);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.GetByIdAsync(id);
            }, "Found deleted Category.");
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(100)]
        public void Test_ThrowsWhenCategoryDoesNotExist(int id)
        {
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.DeleteAsync(id);
            }, "Deleted non-existent Category.");
        }
    }
}